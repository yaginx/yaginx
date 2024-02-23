using Microsoft.Extensions.Primitives;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Yaginx.SimpleProcessors.ConfigProviders;
using Yarp.ReverseProxy.Configuration;

namespace Yaginx.SimpleProcessors;


public class SimpleProcessorConfigManager : EndpointDataSource, IDisposable
{
    private readonly object _syncRoot = new();
    private List<Endpoint>? _endpoints;
    private IChangeToken _endpointsChangeToken;
    private readonly List<Action<EndpointBuilder>> _conventions;
    private readonly SimpleProcessorEndpointFactory _simpleProcessorEndpointFactory;
    private readonly ISimpleProcessorConfigProvider[] _providers;
    private CancellationTokenSource _endpointsChangeSource = new();
    private CancellationTokenSource _configChangeSource = new();
    private readonly ConcurrentDictionary<string, SimpleProcessRouteState> _routes = new(StringComparer.OrdinalIgnoreCase);
    public SimpleProcessorConventionBuilder DefaultBuilder { get; }

    private readonly ConfigState[] _configs;

    public SimpleProcessorConfigManager(SimpleProcessorEndpointFactory simpleProcessorEndpointFactory,
        IEnumerable<ISimpleProcessorConfigProvider> providers)
    {
        _endpointsChangeToken = new CancellationChangeToken(_endpointsChangeSource.Token);
        _conventions = new List<Action<EndpointBuilder>>();
        DefaultBuilder = new SimpleProcessorConventionBuilder(_conventions);
        _simpleProcessorEndpointFactory = simpleProcessorEndpointFactory;

        _providers = providers?.ToArray() ?? throw new ArgumentNullException(nameof(providers));

        if (_providers.Length == 0)
        {
            throw new ArgumentException($"At least one {nameof(ISimpleProcessorConfigProvider)} is required.", nameof(providers));
        }

        _configs = new ConfigState[_providers.Length];
    }

    public override IReadOnlyList<Endpoint> Endpoints
    {
        get
        {
            // The Endpoints needs to be lazy the first time to give a chance to ReverseProxyConventionBuilder to add its conventions.
            // Endpoints are accessed by routing on the first request.
            if (_endpoints is null)
            {
                lock (_syncRoot)
                {
                    if (_endpoints is null)
                    {
                        CreateEndpoints();
                    }
                }
            }
            return _endpoints;
        }
    }

    [MemberNotNull(nameof(_endpoints))]
    private void CreateEndpoints()
    {
        var endpoints = new List<Endpoint>();
        // Directly enumerate the ConcurrentDictionary to limit locking and copying.
        foreach (var existingRoute in _routes)
        {
            // Only rebuild the endpoint for modified routes or clusters.
            var endpoint = existingRoute.Value.CachedEndpoint;
            if (endpoint is null)
            {
                endpoint = _simpleProcessorEndpointFactory.CreateEndpoint(existingRoute.Value.RouteId, existingRoute.Value.PrimaryHost, existingRoute.Value.RelatedHost, 0, _conventions);
                existingRoute.Value.CachedEndpoint = endpoint;
            }
            endpoints.Add(endpoint);
        }
        //var endpoint = _simpleProcessorEndpointFactory.CreateEndpoint("simple_processor_all", 0, _conventions);
        //endpoints.Add(endpoint);

        UpdateEndpoints(endpoints);
    }


    /// <summary>
    /// Applies a new set of ASP .NET Core endpoints. Changes take effect immediately.
    /// </summary>
    /// <param name="endpoints">New endpoints to apply.</param>
    [MemberNotNull(nameof(_endpoints))]
    private void UpdateEndpoints(List<Endpoint> endpoints)
    {
        if (endpoints is null)
        {
            throw new ArgumentNullException(nameof(endpoints));
        }

        lock (_syncRoot)
        {
            // These steps are done in a specific order to ensure callers always see a consistent state.

            // Step 1 - capture old token
            var oldCancellationTokenSource = _endpointsChangeSource;

            // Step 2 - update endpoints
            Volatile.Write(ref _endpoints, endpoints);

            // Step 3 - create new change token
            _endpointsChangeSource = new CancellationTokenSource();
            Volatile.Write(ref _endpointsChangeToken, new CancellationChangeToken(_endpointsChangeSource.Token));

            // Step 4 - trigger old token
            oldCancellationTokenSource?.Cancel();
        }
    }

    public override IChangeToken GetChangeToken() => Volatile.Read(ref _endpointsChangeToken);

    internal async Task<EndpointDataSource> InitialLoadAsync()
    {
        // Trigger the first load immediately and throw if it fails.
        // We intend this to crash the app so we don't try listening for further changes.
        try
        {
            var routes = new List<RequestMetadataConfig>();

            // Begin resolving config providers concurrently.
            var resolvedConfigs = new List<(int Index, ISimpleProcessorConfigProvider Provider, ValueTask<ISimpleProcessorConfig> Config)>(_providers.Length);
            for (var i = 0; i < _providers.Length; i++)
            {
                var provider = _providers[i];
                var configLoadTask = LoadConfigAsync(provider, cancellationToken: default);
                resolvedConfigs.Add((i, provider, configLoadTask));
            }

            // Wait for all configs to be resolved.
            foreach (var (i, provider, configLoadTask) in resolvedConfigs)
            {
                var config = await configLoadTask;
                _configs[i] = new ConfigState(provider, config);
                routes.AddRange(config.Routes ?? Array.Empty<RequestMetadataConfig>());
                //clusters.AddRange(config.Clusters ?? Array.Empty<ClusterConfig>());
            }

            var proxyConfigs = ExtractListOfProxyConfigs(_configs);

            //foreach (var configChangeListener in _configChangeListeners)
            //{
            //    configChangeListener.ConfigurationLoaded(proxyConfigs);
            //}

            await ApplyConfigAsync(routes);

            //foreach (var configChangeListener in _configChangeListeners)
            //{
            //    configChangeListener.ConfigurationApplied(proxyConfigs);
            //}

            //ListenForConfigChanges();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to load or apply the proxy configuration.", ex);
        }

        // Initial active health check is run in the background.
        // Directly enumerate the ConcurrentDictionary to limit locking and copying.
        //_ = _activeHealthCheckMonitor.CheckHealthAsync(_clusters.Select(pair => pair.Value));
        return this;
    }
    private async Task<bool> ApplyConfigAsync(IReadOnlyList<RequestMetadataConfig> routes)
    {
        var (configuredRoutes, routeErrors) = await VerifyRoutesAsync(routes, cancellation: default);

        if (routeErrors.Count > 0)
        {
            throw new AggregateException("The proxy config is invalid.", routeErrors);
        }

        // Update clusters first because routes need to reference them.
        var routesChanged = UpdateRuntimeRoutes(configuredRoutes);
        return routesChanged;
    }
    private bool UpdateRuntimeRoutes(IList<RequestMetadataConfig> incomingRoutes)
    {
        var desiredRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var changed = false;

        foreach (var incomingRoute in incomingRoutes)
        {
            desiredRoutes.Add(incomingRoute.RouteId);

            // Note that this can be null, and that is fine. The resulting route may match
            // but would then fail to route, which is exactly what we were instructed to do in this case
            // since no valid cluster was specified.
            //_clusters.TryGetValue(incomingRoute.ClusterId ?? string.Empty, out var cluster);

            if (_routes.TryGetValue(incomingRoute.RouteId, out var currentRoute))
            {
                //if (currentRoute.Model.HasConfigChanged(incomingRoute, cluster, currentRoute.ClusterRevision))
                //{
                //    currentRoute.CachedEndpoint = null; // Recreate endpoint
                //    var newModel = BuildRouteModel(incomingRoute, cluster);
                //    currentRoute.Model = newModel;
                //    currentRoute.ClusterRevision = cluster?.Revision;
                //    changed = true;
                //    Log.RouteChanged(_logger, currentRoute.RouteId);
                //}
            }
            else
            {
                //var newModel = BuildRouteModel(incomingRoute, cluster);
                var newState = new SimpleProcessRouteState()
                {
                    RouteId = incomingRoute.RouteId
                    //Model = newModel,
                    //ClusterRevision = cluster?.Revision,
                };
                var added = _routes.TryAdd(newState.RouteId, newState);
                Debug.Assert(added);
                changed = true;
                //Log.RouteAdded(_logger, newState.RouteId);
            }
        }

        // Directly enumerate the ConcurrentDictionary to limit locking and copying.
        foreach (var existingRoutePair in _routes)
        {
            var routeId = existingRoutePair.Value.RouteId;
            if (!desiredRoutes.Contains(routeId))
            {
                // NOTE 1: Remove is safe to do within the `foreach` loop on ConcurrentDictionary
                //
                // NOTE 2: Removing the route from _routes is safe and existing
                // ASP.NET Core endpoints will continue to work with their existing behavior since
                // their copy of `RouteModel` is immutable and remains operational in whichever state is was in.
                //Log.RouteRemoved(_logger, routeId);
                var removed = _routes.TryRemove(routeId, out var _);
                Debug.Assert(removed);
                changed = true;
            }
        }

        return changed;
    }
    private async Task<(IList<RequestMetadataConfig>, IList<Exception>)> VerifyRoutesAsync(IReadOnlyList<RequestMetadataConfig> routes, CancellationToken cancellation)
    {
        if (routes is null)
        {
            return (Array.Empty<RequestMetadataConfig>(), Array.Empty<Exception>());
        }

        var seenRouteIds = new HashSet<string>(routes.Count, StringComparer.OrdinalIgnoreCase);
        var configuredRoutes = new List<RequestMetadataConfig>(routes.Count);
        var errors = new List<Exception>();

        foreach (var r in routes)
        {
            if (seenRouteIds.Contains(r.RouteId))
            {
                errors.Add(new ArgumentException($"Duplicate route '{r.RouteId}'"));
                continue;
            }

            var route = r;

            //try
            //{
            //    if (_filters.Length != 0)
            //    {
            //        ClusterConfig? cluster = null;
            //        if (route.ClusterId is not null)
            //        {
            //            clusters.TryGetValue(route.ClusterId, out cluster);
            //        }

            //        foreach (var filter in _filters)
            //        {
            //            route = await filter.ConfigureRouteAsync(route, cluster, cancellation);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    errors.Add(new Exception($"An exception was thrown from the configuration callbacks for route '{r.RouteId}'.", ex));
            //    continue;
            //}

            //var routeErrors = await _configValidator.ValidateRouteAsync(route);
            //if (routeErrors.Count > 0)
            //{
            //    errors.AddRange(routeErrors);
            //    continue;
            //}

            seenRouteIds.Add(route.RouteId);
            configuredRoutes.Add(route);
        }

        if (errors.Count > 0)
        {
            return (Array.Empty<RequestMetadataConfig>(), errors);
        }
        await Task.CompletedTask;
        return (configuredRoutes, errors);
    }
    private static IReadOnlyList<ISimpleProcessorConfig> ExtractListOfProxyConfigs(IEnumerable<ConfigState> configStates)
    {
        return configStates.Select(state => state.LatestConfig).ToList().AsReadOnly();
    }
    //private void ListenForConfigChanges()
    //{
    //    // Use a central change token to avoid overlap between different sources.
    //    var source = new CancellationTokenSource();
    //    _configChangeSource = source;
    //    var poll = false;

    //    foreach (var configState in _configs)
    //    {
    //        if (configState.LoadFailed)
    //        {
    //            // We can't register for change notifications if the last load failed.
    //            poll = true;
    //            continue;
    //        }

    //        configState.CallbackCleanup?.Dispose();
    //        var token = configState.LatestConfig.ChangeToken;
    //        if (token.ActiveChangeCallbacks)
    //        {
    //            configState.CallbackCleanup = token.RegisterChangeCallback(SignalChange, source);
    //        }
    //        else
    //        {
    //            poll = true;
    //        }
    //    }

    //    if (poll)
    //    {
    //        source.CancelAfter(TimeSpan.FromMinutes(5));
    //    }

    //    // Don't register until we're done hooking everything up to avoid cancellation races.
    //    source.Token.Register(ReloadConfig, this);

    //    static void SignalChange(object? obj)
    //    {
    //        var token = (CancellationTokenSource)obj!;
    //        try
    //        {
    //            token.Cancel();
    //        }
    //        // Don't throw if the source was already disposed.
    //        catch (ObjectDisposedException) { }
    //    }

    //    static void ReloadConfig(object? state)
    //    {
    //        var manager = (ProxyConfigManager)state!;
    //        _ = manager.ReloadConfigAsync();
    //    }
    //}
    public void Dispose()
    {
        _configChangeSource.Dispose();
        //foreach (var instance in _configs)
        //{
        //    instance?.CallbackCleanup?.Dispose();
        //}
    }

    private ValueTask<ISimpleProcessorConfig> LoadConfigAsync(ISimpleProcessorConfigProvider provider, CancellationToken cancellationToken)
    {
        var config = provider.GetConfig();
        //ValidateConfigProperties(config);

        //if (_destinationResolver.GetType() == typeof(NoOpDestinationResolver))
        //{
        //    return new(config);
        //}

        return LoadConfigAsyncCore(config, cancellationToken);
    }

    private async ValueTask<ISimpleProcessorConfig> LoadConfigAsyncCore(ISimpleProcessorConfig config, CancellationToken cancellationToken)
    {
        //List<(int Index, ValueTask<ResolvedDestinationCollection> Task)> resolverTasks = new();
        //List<ClusterConfig> clusters = new(config.Clusters);
        List<IChangeToken>? changeTokens = null;
        //for (var i = 0; i < clusters.Count; i++)
        //{
        //    var cluster = clusters[i];
        //    if (cluster.Destinations is { Count: > 0 } destinations)
        //    {
        //        // Resolve destinations if there are any.
        //        var task = _destinationResolver.ResolveDestinationsAsync(destinations, cancellationToken);
        //        resolverTasks.Add((i, task));
        //    }
        //}

        //if (resolverTasks.Count > 0)
        //{
        //    foreach (var (i, task) in resolverTasks)
        //    {
        //        ResolvedDestinationCollection resolvedDestinations;
        //        try
        //        {
        //            resolvedDestinations = await task;
        //        }
        //        catch (Exception exception)
        //        {
        //            var cluster = clusters[i];
        //            throw new InvalidOperationException($"Error resolving destinations for cluster {cluster.ClusterId}", exception);
        //        }

        //        clusters[i] = clusters[i] with { Destinations = resolvedDestinations.Destinations };
        //        if (resolvedDestinations.ChangeToken is { } token)
        //        {
        //            changeTokens ??= new();
        //            changeTokens.Add(token);
        //        }
        //    }

        //    IChangeToken changeToken;
        //    if (changeTokens is not null)
        //    {
        //        // Combine change tokens from the resolver with the configuration's existing change token.
        //        changeTokens.Add(config.ChangeToken);
        //        changeToken = new CompositeChangeToken(changeTokens);
        //    }
        //    else
        //    {
        //        changeToken = config.ChangeToken;
        //    }

        //    // Return updated config
        //    return new ResolvedProxyConfig(config, clusters, changeToken);
        //}
        await Task.CompletedTask;
        return config;
    }
    private sealed class ResolvedSimpleProcessConfig : ISimpleProcessorConfig
    {
        private readonly ISimpleProcessorConfig _innerConfig;

        public ResolvedSimpleProcessConfig(ISimpleProcessorConfig innerConfig, IChangeToken changeToken)
        {
            _innerConfig = innerConfig;
            //Clusters = clusters;
            ChangeToken = changeToken;
        }

        public IReadOnlyList<RequestMetadataConfig> Routes => _innerConfig.Routes;
        //public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }
    }
    private class ConfigState
    {
        public ConfigState(ISimpleProcessorConfigProvider provider, ISimpleProcessorConfig config)
        {
            Provider = provider;
            LatestConfig = config;
        }

        public ISimpleProcessorConfigProvider Provider { get; }

        public ISimpleProcessorConfig LatestConfig { get; set; }

        public bool LoadFailed { get; set; }

        public IDisposable? CallbackCleanup { get; set; }
    }
}

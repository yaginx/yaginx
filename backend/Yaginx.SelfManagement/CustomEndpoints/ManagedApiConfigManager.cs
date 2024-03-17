using AgileLabs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Yaginx.SelfManagement.ApiDescriptions;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

namespace Yaginx.SelfManagement.CustomEndpoints
{

    public class ManagedApiConfigManager : EndpointDataSource
    {
        private readonly object _syncRoot = new();
        private List<Endpoint> _endpoints;
        private IChangeToken _endpointsChangeToken;
        private readonly List<Action<EndpointBuilder>> _conventions;
        private readonly ManagedApiEndpointFactory _managedApiEndpointFactory;
        private CancellationTokenSource _endpointsChangeSource = new();
        private readonly ConcurrentDictionary<string, ManagedApiRouteState> _routes = new(StringComparer.OrdinalIgnoreCase);
        public ManagedApiConventionBuilder DefaultBuilder { get; }

        //private readonly ConfigState[] _configs;

        public ManagedApiConfigManager(ManagedApiEndpointFactory managedApiEndpointFactory)
        {
            _endpointsChangeToken = new CancellationChangeToken(_endpointsChangeSource.Token);
            _conventions = new List<Action<EndpointBuilder>>();
            DefaultBuilder = new ManagedApiConventionBuilder(_conventions);
            _managedApiEndpointFactory = managedApiEndpointFactory;
        }
        public override IReadOnlyList<Endpoint> Endpoints
        {
            get
            {
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
                if (!existingRoute.Value.Metadata.TryGetValue("RawModel", out var managedApiObjValue))
                {
                    continue;
                }

                var managedApiInfo = (ManagedApiServiceInfo)managedApiObjValue;
                //if (managedApiInfo.Brand.IsNullOrEmpty() || managedApiInfo.Group.IsNullOrEmpty())
                //{
                //    continue;
                //}

                // Only rebuild the endpoint for modified routes or clusters.
                var endpoint = existingRoute.Value.CachedEndpoint;
                if (endpoint is null)
                {
                    var model = new ManagedApiMetadataModel
                    {
                        Brand = managedApiInfo.Brand,
                        Group = managedApiInfo.Group,
                        ServiceName = managedApiInfo.ServiceName,
                        ServiceType = managedApiInfo.ServiceType,
                        Metadata = existingRoute.Value.Metadata,
                    };
                    endpoint = _managedApiEndpointFactory.CreateManagedApiEndpoint($"managedapi-{existingRoute.Value.RouteId}", model, 0, _conventions);
                    existingRoute.Value.CachedEndpoint = endpoint;
                }
                endpoints.Add(endpoint);
            }
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
            try
            {
                var defaultModel = new ManagedApiServiceInfo { };
                var defaultRoute = new ManagedApiRouteState
                {
                    RouteId = "yaingx-console-route",
                    Metadata = new Dictionary<string, object> { { "RawModel", defaultModel } }
                };
                _routes.TryAdd(defaultRoute.RouteId, defaultRoute);

                using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
                var apiDescription = scope.WorkContext.ServiceProvider.GetRequiredService<IManagedApiDescription>();

                var managedApiServices = apiDescription.GetManagedApiServiceList();

                foreach (var apiService in managedApiServices)
                {
                    var newState = new ManagedApiRouteState()
                    {
                        RouteId = $"{apiService.Brand}-{apiService.Group}-{apiService.ServiceName}",
                        Metadata = new Dictionary<string, object> { { "RawModel", apiService } }
                    };
                    _routes.TryAdd(newState.RouteId, newState);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to load or apply the proxy configuration.", ex);
            }
            return this;
        }
    }
}

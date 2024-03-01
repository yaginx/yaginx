using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using Yaginx.DomainModels;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Yaginx.Infrastructure.ProxyConfigProviders
{
    /// <summary>
    /// 服务提供基础类
    /// </summary>
    public class FileProxyConfigProvider : IProxyConfigProvider, IDisposable
    {
        private ProxyConfig _config;
        private CancellationTokenSource _changeToken;
        private RouteChangeToken _reloadToken;
        private bool _disposed;
        private IDisposable _subscription;
        private ILogger<FileProxyConfigProvider> _Logger;
        private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;
        private readonly ProxyRuleRedisStorageService _proxyRuleRedisStorageService;

        public FileProxyConfigProvider(
            ILogger<FileProxyConfigProvider> logger,
            ProxyRuleChangeNotifyService proxyRuleChangeNotifyService,
            ProxyRuleRedisStorageService proxyRuleRedisStorageService)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
            _proxyRuleRedisStorageService = proxyRuleRedisStorageService;
            _changeToken = new CancellationTokenSource();
            _reloadToken = new RouteChangeToken();

            ChangeToken.OnChange(() => _proxyRuleChangeNotifyService.CreateProxyChanageToken(), Reload);
        }

        public IProxyConfig GetConfig()
        {
            if (_config == null)
            {
                _subscription = ChangeToken.OnChange(() => _reloadToken, UpdateConfig);
                UpdateConfig();
            }
            return _config;
        }

        [MemberNotNull(nameof(_config))]
        private void UpdateConfig()
        {
            _config = new ProxyConfig();

            try
            {
                _Logger.LogInformation(0, "Start load ReverseProxy Config");
                var RuleConfigs = LoadRedisDyamicRules().ToList();
                _config.Routes.AddRange(RuleConfigs.Select(x => x.Item1));
                _config.Clusters.AddRange(RuleConfigs.Select(x => x.Item2));

                var databaseConfigs = LoadDatabaseRules().ToList();

                _config.Routes.AddRange(databaseConfigs.Select(x => x.Item1));
                _config.Clusters.AddRange(databaseConfigs.Select(x => x.Item2));

                CancellationTokenSource mOldChangeToken = _changeToken;
                _changeToken = new CancellationTokenSource();
                _config.ChangeToken = new CancellationChangeToken(_changeToken.Token);
                mOldChangeToken?.Cancel(throwOnFirstException: false);
                _Logger.LogInformation(0, "Success load ReverseProxy Config");
            }
            catch (Exception ex)
            {
                _Logger.LogError(0, ex, "UpdateConfig Error");
            }
        }

        private IEnumerable<(RouteConfig, ClusterConfig)> LoadRedisDyamicRules()
        {
            var rules = _proxyRuleRedisStorageService.GetRules().Result;
            foreach (var rule in rules)
            {
                if (string.IsNullOrEmpty(rule.RequestPattern))
                    continue;

                if ((rule.Clusters?.Count ?? 0) == 0)
                    continue;

                string mId = rule.RequestPattern.ToLower();
                var dicClusters = rule.Clusters.Select(x => new KeyValuePair<string, string>($"{mId}_{x.ClusterId}", x.Address)).ToDictionary(key => key.Key, value => value.Value);
                yield return BuildRouteAndClusterConfig(mId, rule.RequestPattern, dicClusters);
            }

            (RouteConfig, ClusterConfig) BuildRouteAndClusterConfig(string mId, string requestPattern, Dictionary<string, string> clusters)
            {
                RouteConfig routeConfig = new RouteConfig()
                {
                    RouteId = mId,
                    ClusterId = mId,
                    Match = new RouteMatch
                    {
                        Path = $"/{requestPattern}/{{**remainder}}"
                    }
                }.WithTransformUseOriginalHostHeader(useOriginal: false);


                Dictionary<string, DestinationConfig> mDestinationConfigs = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in clusters)
                {
                    mDestinationConfigs.Add(item.Key, new DestinationConfig { Address = item.Value });
                }

                var clusterConfig = new ClusterConfig()
                {
                    ClusterId = mId,
                    LoadBalancingPolicy = "PowerOfTwoChoices",
                    HttpRequest = new Yarp.ReverseProxy.Forwarder.ForwarderRequestConfig { ActivityTimeout = new TimeSpan(0, 0, 15, 0, 0) },
                    Destinations = mDestinationConfigs,
                    //HttpClient = new HttpClientConfig { WebProxy = new WebProxyConfig() { Address = new Uri("http://localhost:8888") } }
                };
                return (routeConfig, clusterConfig);
            }
        }

        private IEnumerable<(RouteConfig, ClusterConfig)> LoadDatabaseRules()
        {
            using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
            var _websiteRepository = scope.WorkContext.Resolve<IWebsiteRepository>();
            var websites = _websiteRepository.SearchAsync().Result;
            foreach (var website in websites)
            {
                var spec = website.Specifications;
                if (string.IsNullOrEmpty(spec.DefaultHost) || string.IsNullOrEmpty(spec.DefaultDestination))
                    continue;

                var websiteId = website.Id.Value.ToString();

                var hosts = new List<string>();

                if (spec.DefaultHost.IsNotNullOrWhitespace())
                {
                    var normalizedHost = spec.DefaultHost.ToLower();
                    hosts.Add(normalizedHost);
                }

                foreach (var host in website.Hosts)
                {
                    var normalizedHost = host.ToLower();
                    if (!hosts.Contains(normalizedHost))
                    {
                        hosts.Add(normalizedHost);
                    }
                }

                if (!website.ProxyRules.IsNullOrEmpty())
                {
                    // 加载自定义的Rules
                    foreach (var proxyRule in website.ProxyRules)
                    {
                        var routeId = $"router_{websiteId}_{proxyRule.PathPattern.ToLower()}";
                        var clusterId = $"cluster_{websiteId}_{proxyRule.PathPattern.ToLower()}";

                        RouteConfig routeConfig = new RouteConfig()
                        {
                            RouteId = routeId,
                            ClusterId = clusterId,
                            Match = new RouteMatch
                            {
                                Hosts = hosts,
                                Path = proxyRule.PathPattern
                            }
                        };

                        Dictionary<string, DestinationConfig> mDestinationConfigs = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);
                        if (!proxyRule.Destinations.IsNullOrEmpty())
                        {
                            foreach (var dest in proxyRule.Destinations)
                            {
                                if (!mDestinationConfigs.TryAdd(dest.Name, new DestinationConfig { Address = dest.Address }))
                                {
                                    _Logger.LogWarning($"{website.Name} pattern [{proxyRule.PathPattern}] destination {dest.Name} add fail(Duplicated) with value {dest.Address}");
                                }
                            }
                        }

                        var clusterConfig = new ClusterConfig()
                        {
                            ClusterId = clusterId,
                            LoadBalancingPolicy = "PowerOfTwoChoices",
                            HttpRequest = new Yarp.ReverseProxy.Forwarder.ForwarderRequestConfig { ActivityTimeout = new TimeSpan(0, 0, 15, 0, 0) },
                            Destinations = mDestinationConfigs,
                            //HttpClient = new HttpClientConfig
                            //{
                            //    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                            //    WebProxy = new WebProxyConfig()
                            //    {
                            //        Address = new Uri("http://localhost:8888")
                            //    }
                            //}
                        };
                        yield return (routeConfig, clusterConfig);
                    }
                }
                else
                {
                    // 加载默认的配置
                    var routeId = $"router_{websiteId}";
                    var clusterId = $"cluster_{websiteId}";
                    RouteConfig routeConfig = new RouteConfig()
                    {
                        RouteId = routeId,
                        ClusterId = clusterId,
                        Match = new RouteMatch
                        {
                            Hosts = hosts
                        }
                    }
                    .WithTransformUseOriginalHostHeader(useOriginal: spec.IsWithOriginalHostHeader);

                    if (spec.DefaultDestinationHost.IsNotNullOrWhitespace())
                    {
                        routeConfig = routeConfig.WithTransformRequestHeader("Host", spec.DefaultDestinationHost, false);
                    }

                    if (!website.ProxyTransforms.IsNullOrEmpty())
                    {
                        routeConfig = routeConfig.WithTransform((dic) =>
                        {
                            foreach (var item in website.ProxyTransforms)
                            {
                                if (!dic.TryAdd(item.Key, item.Value))
                                {
                                    _Logger.LogWarning($"{website.Name} ProxyTransform item {item} add fail with value [{item.Value}]");
                                }
                            }
                        });
                    }
     
                    Dictionary<string, DestinationConfig> mDestinationConfigs = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "default", new DestinationConfig { Address = spec.DefaultDestination } }
                    };

                    Uri webProxyAddress = null;
                    if (spec.WebProxy.IsNotNullOrWhitespace())
                    {
                        webProxyAddress = new Uri(spec.WebProxy);
                    }

                    var clusterConfig = new ClusterConfig()
                    {
                        ClusterId = clusterId,
                        LoadBalancingPolicy = "PowerOfTwoChoices",
                        HttpRequest = new Yarp.ReverseProxy.Forwarder.ForwarderRequestConfig { ActivityTimeout = new TimeSpan(0, 0, 15, 0, 0) },
                        Destinations = mDestinationConfigs,
                        HttpClient = new HttpClientConfig
                        {
                            WebProxy = new WebProxyConfig() { Address = webProxyAddress },
                            DangerousAcceptAnyServerCertificate = spec.IsAllowUnsafeSslCertificate,
                        }
                    };
                    yield return (routeConfig, clusterConfig);
                }
            }
        }

        protected virtual void Reload() => Interlocked.Exchange(ref _reloadToken, new RouteChangeToken()).OnReload();

        public void Dispose()
        {
            if (!_disposed)
            {
                _subscription?.Dispose();
                _changeToken?.Dispose();
                _disposed = true;
            }
        }
    }
}

using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
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

			ChangeToken.OnChange(() => _proxyRuleChangeNotifyService.CreateChanageToken(), Reload);
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

			var RuleConfigs = LoadDyamicRules();
			_config.Routes.AddRange(RuleConfigs.Select(x => x.Item1));
			_config.Clusters.AddRange(RuleConfigs.Select(x => x.Item2));

			CancellationTokenSource mOldChangeToken = _changeToken;
			_changeToken = new CancellationTokenSource();
			_config.ChangeToken = new CancellationChangeToken(_changeToken.Token);
			mOldChangeToken?.Cancel(throwOnFirstException: false);
		}

		private IEnumerable<(RouteConfig, ClusterConfig)> LoadDyamicRules()
		{
			var Rules = _proxyRuleRedisStorageService.GetRules().Result;
			foreach (var rule in Rules)
			{
				if (string.IsNullOrEmpty(rule.RequestPattern))
					continue;

				if ((rule.Clusters?.Count ?? 0) == 0)
					continue;

				string mId = rule.RequestPattern.ToLower();
				var dicClusters = rule.Clusters.Select(x => new KeyValuePair<string, string>($"{mId}_{x.Key}", x.Value)).ToDictionary(key => key.Key, value => value.Value);
				yield return BuildRouteAndClusterConfig(mId, rule.RequestPattern, dicClusters);
			}
		}

		private (RouteConfig, ClusterConfig) BuildRouteAndClusterConfig(string mId, string requestPattern, Dictionary<string, string> clusters)
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

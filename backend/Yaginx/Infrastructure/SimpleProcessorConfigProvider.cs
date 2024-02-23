using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using Yaginx.DomainModels;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.SimpleProcessors.ConfigProviders;
using Yarp.ReverseProxy.Configuration;

namespace Yaginx.Infrastructure
{
    public class SimpleProcessorConfigProvider : ISimpleProcessorConfigProvider
    {
        private SimpleProcessConfig _config;
        private CancellationTokenSource _changeToken;
        private RouteChangeToken _reloadToken;
        private bool _disposed;
        private IDisposable _subscription;
        private ILogger<FileProxyConfigProvider> _Logger;
        private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;
        private readonly ProxyRuleRedisStorageService _proxyRuleRedisStorageService;
        private readonly IWebsiteRepository _websiteRepository;

        public SimpleProcessorConfigProvider(
            ILogger<FileProxyConfigProvider> logger,
            ProxyRuleChangeNotifyService proxyRuleChangeNotifyService,
            ProxyRuleRedisStorageService proxyRuleRedisStorageService,
            IWebsiteRepository websiteRepository)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
            _proxyRuleRedisStorageService = proxyRuleRedisStorageService;
            _websiteRepository = websiteRepository;
            _changeToken = new CancellationTokenSource();
            _reloadToken = new RouteChangeToken();

            ChangeToken.OnChange(() => _proxyRuleChangeNotifyService.CreateChanageToken(), Reload);
        }
        public ISimpleProcessorConfig GetConfig()
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
            _config = new SimpleProcessConfig();

            try
            {
                _Logger.LogInformation(0, "Start load ReverseProxy Config");
                //var RuleConfigs = LoadRedisDyamicRules().ToList();
                //_config.Routes.AddRange(RuleConfigs.Select(x => x.Item1));
                //_config.Clusters.AddRange(RuleConfigs.Select(x => x.Item2));

                //var databaseConfigs = LoadDatabaseRules().ToList();

                //_config.Routes.AddRange(databaseConfigs.Select(x => x.Item1));
                //_config.Clusters.AddRange(databaseConfigs.Select(x => x.Item2));

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

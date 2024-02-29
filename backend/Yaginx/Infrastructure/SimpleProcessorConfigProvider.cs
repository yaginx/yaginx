using Microsoft.Extensions.Primitives;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Yaginx.DomainModels;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.SimpleProcessors.ConfigProviders;

namespace Yaginx.Infrastructure
{
    public class SimpleProcessorConfigProvider : ISimpleProcessorConfigProvider
    {
        private SimpleProcessConfig _config;
        private CancellationTokenSource _changeToken;
        private RouteChangeToken _reloadToken;
        private bool _disposed;
        private IDisposable _subscription;
        private ILogger<SimpleProcessorConfigProvider> _Logger;
        private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;
        private readonly IWebsiteRepository _websiteRepository;

        public SimpleProcessorConfigProvider(
            ILogger<SimpleProcessorConfigProvider> logger,
            ProxyRuleChangeNotifyService proxyRuleChangeNotifyService,
            IWebsiteRepository websiteRepository)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
            _websiteRepository = websiteRepository;
            _changeToken = new CancellationTokenSource();
            _reloadToken = new RouteChangeToken();

            ChangeToken.OnChange(() => _proxyRuleChangeNotifyService.CreateSimpleProcessorChanageToken(), Reload);
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
                _Logger.LogInformation(0, "Start load SimpleProcessor Config");
                //var RuleConfigs = LoadRedisDyamicRules().ToList();
                //_config.Routes.AddRange(RuleConfigs.Select(x => x.Item1));
                //_config.Clusters.AddRange(RuleConfigs.Select(x => x.Item2));

                var databaseConfigs = LoadDatabaseRules().ToList();

                //_config.Routes.AddRange(databaseConfigs.Select(x => x.Item1));
                //_config.Clusters.AddRange(databaseConfigs.Select(x => x.Item2));

                _config.WebSites.AddRange(databaseConfigs);

                CancellationTokenSource mOldChangeToken = _changeToken;
                _changeToken = new CancellationTokenSource();
                _config.ChangeToken = new CancellationChangeToken(_changeToken.Token);
                mOldChangeToken?.Cancel(throwOnFirstException: false);
                _Logger.LogInformation(0, "Success load SimpleProcessor Config");
            }
            catch (Exception ex)
            {
                _Logger.LogError(0, ex, "UpdateConfig Error");
            }
        }
        private IEnumerable<WebSiteMetadataConfig> LoadDatabaseRules()
        {
            var websites = _websiteRepository.SearchAsync().Result;
            foreach (var website in websites)
            {
                var spec = website.Specifications;
                if (!spec.IsAutoRedirectHttp2Https)
                    continue;

                if (string.IsNullOrEmpty(spec.DefaultHost))
                    continue;

                var websiteId = website.Id.Value.ToString();
                var metadata = new Dictionary<string, object>();
                metadata.Add("RawModel", website);

                WebSiteMetadataConfig config = new WebSiteMetadataConfig()
                {
                    RouteId = websiteId,
                    Metadata = new ReadOnlyDictionary<string, object>(metadata)
                };

                config.PrimaryHost = spec.DefaultHost.ToLower();

                var relatedHost = new List<string>(website.Hosts.Length);
                foreach (var host in website.Hosts)
                {
                    var normalizedHost = host.ToLower();
                    if (!relatedHost.Contains(normalizedHost))
                    {
                        relatedHost.Add(normalizedHost);
                    }
                }
                config.RelatedHost = relatedHost.ToArray();
                yield return config;
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

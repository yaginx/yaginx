using LettuceEncrypt.Internal.AcmeStates;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Yaginx.YaginxAcmeLoaders
{
    /// <summary>
    /// AcmeCertificateLoader
    /// </summary>
    public class YaginxAcmeCertificateLoader : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        private readonly IServer _server;
        private readonly IConfiguration _config;
        private readonly ICertificateDomainRepsitory _certificateDomainRepsitory;

        public YaginxAcmeCertificateLoader(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<YaginxAcmeCertificateLoader> logger,
            IServer server,
            IConfiguration config,
            ICertificateDomainRepsitory certificateDomainRepsitory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _server = server;
            _config = config;
            _certificateDomainRepsitory = certificateDomainRepsitory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_server.GetType().Name.StartsWith(nameof(KestrelServer)))
            {
                var serverType = _server.GetType().FullName;
                _logger.LogWarning(
                    "LettuceEncrypt can only be used with Kestrel and is not supported on {serverType} servers. Skipping certificate provisioning.",
                    serverType);
                return;
            }

            if (_config.GetValue<bool>("UseIISIntegration"))
            {
                _logger.LogWarning(
                    "LettuceEncrypt does not work with apps hosting in IIS. IIS does not allow for dynamic HTTPS certificate binding." +
                    "Skipping certificate provisioning.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var waitSconds = RandomNumberGenerator.GetInt32(30, 120);// 每隔30~120秒检查一次
                await Task.Delay(TimeSpan.FromSeconds(waitSconds), stoppingToken);

                try
                {
                    var domains = await _certificateDomainRepsitory.GetFreeCertDomainAsync();
                    foreach (var domain in domains)
                    {
                        await ProcessDomain(domain, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(0, ex, "Certificate Loader Error");
                }
            }
        }

        private async Task ProcessDomain(string domain, CancellationToken stoppingToken)
        {
            var acmeStateMachineScope = _serviceScopeFactory.CreateScope();
            try
            {
                IAcmeState state = acmeStateMachineScope.ServiceProvider.GetRequiredService<YaginxServerStartupState>();
                if (state is AcmeState acmeState)
                {
                    acmeState.Context.CurrentDomain = domain;
                    acmeState.Context.EmailAddress = "duke@feinian.net";
                }

                while (state.ContinueState == AcmeStateContinueStatus.Continue && !stoppingToken.IsCancellationRequested)
                {
                    //if (state.ContinueState == AcmeStateContinueStatus.Continue)
                    //{
                    //    _logger.LogInformation($"Apply cert SUCCESS for domain {domain}!");
                    //    break;
                    //}
                    _logger.LogTrace("ACME state transition: moving to {stateName}", state.GetType().Name);
                    state = await state.MoveNextAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("State machine cancellation requested. Exiting...");
            }
            catch (AggregateException ex) when (ex.InnerException != null)
            {
                await _certificateDomainRepsitory.UnFreeDomainAsync(domain, ex.InnerException.Message);
                _logger.LogError(0, ex.InnerException, "ACME state machine encountered unhandled error");
            }
            catch (Exception ex)
            {
                await _certificateDomainRepsitory.UnFreeDomainAsync(domain, ex.Message);
                _logger.LogError(0, ex, "ACME state machine encountered unhandled error");
            }
        }
    }
}

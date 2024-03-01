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

        public YaginxAcmeCertificateLoader(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<YaginxAcmeCertificateLoader> logger,
            IServer server,
            IConfiguration config)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _server = server;
            _config = config;
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
                using var acmeStateMachineScopeRoot = _serviceScopeFactory.CreateScope();
                var _domainRepsitory = acmeStateMachineScopeRoot.ServiceProvider.GetRequiredService<ICertificateDomainRepsitory>();
                var domains = await _domainRepsitory.GetFreeCertDomainAsync();
                foreach (var domain in domains)
                {
                    using var acmeStateMachineScope = _serviceScopeFactory.CreateScope();

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
                        await _domainRepsitory.UnFreeDomainAsync(domain, ex.InnerException.Message);
                        _logger.LogError(0, ex.InnerException, "ACME state machine encountered unhandled error");
                    }
                    catch (Exception ex)
                    {
                        await _domainRepsitory.UnFreeDomainAsync(domain, ex.Message);
                        _logger.LogError(0, ex, "ACME state machine encountered unhandled error");
                    }
                }
            }
        }
    }
}

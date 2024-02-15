using LettuceEncrypt;
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
        private readonly IOptions<LettuceEncryptOptions> _options;
        private readonly ILogger _logger;

        private readonly IServer _server;
        private readonly IConfiguration _config;
        private readonly ICertificateDomainRepsitory _domainRepsitory;

        public YaginxAcmeCertificateLoader(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<YaginxAcmeCertificateLoader> logger,
            IServer server,
            IConfiguration config,
            ICertificateDomainRepsitory domainRepsitory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _server = server;
            _config = config;
            _domainRepsitory = domainRepsitory;
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
                await Task.Delay(TimeSpan.FromSeconds(30));
                var domains = _domainRepsitory.GetFreeCertDomain();
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

                        while (!stoppingToken.IsCancellationRequested)
                        {
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
                        _domainRepsitory.UpdateDomainStatus(domain, ex.InnerException.Message);
                        _logger.LogError(0, ex.InnerException, "ACME state machine encountered unhandled error");
                    }
                    catch (Exception ex)
                    {
                        _domainRepsitory.UpdateDomainStatus(domain, ex.Message);
                        _logger.LogError(0, ex, "ACME state machine encountered unhandled error");
                    }
                }
            }
        }
    }
}

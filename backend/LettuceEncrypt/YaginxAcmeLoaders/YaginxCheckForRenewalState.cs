using LettuceEncrypt;
using LettuceEncrypt.Internal;
using LettuceEncrypt.Internal.AcmeStates;
using LettuceEncrypt.Internal.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Yaginx.YaginxAcmeLoaders
{
    internal class YaginxCheckForRenewalState : AcmeState
    {
        private readonly ILogger<YaginxCheckForRenewalState> _logger;
        private readonly CertificateSelector _selector;
        private readonly IClock _clock;

        public YaginxCheckForRenewalState(
            AcmeStateMachineContext context,
            ILogger<YaginxCheckForRenewalState> logger,
            CertificateSelector selector,
            IClock clock) : base(context)
        {
            _logger = logger;
            _selector = selector;
            _clock = clock;
        }

        public override async Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var checkPeriod = Context.RenewalCheckPeriod;
                var daysInAdvance = Context.RenewDaysInAdvance;
                if (!checkPeriod.HasValue || !daysInAdvance.HasValue)
                {
                    _logger.LogInformation("Automatic certificate renewal is not configured. Stopping {service}",
                        nameof(AcmeCertificateLoader));
                    return MoveTo<TerminalState>();
                }

                var domainNames = new string[] { Context.CurrentDomain };
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Checking certificates' renewals for {hostname}",
                        string.Join(", ", domainNames));
                }

                foreach (var domainName in domainNames)
                {
                    if (!_selector.TryGet(domainName, out var cert)
                        || cert == null
                        || cert.NotAfter <= _clock.Now.DateTime + daysInAdvance.Value)
                    {
                        return MoveTo<YaginxBeginCertificateCreationState>();
                    }
                }
            }
            return MoveTo<TerminalState>();
        }
    }
}

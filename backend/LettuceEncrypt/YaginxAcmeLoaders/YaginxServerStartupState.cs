using LettuceEncrypt.Internal;
using LettuceEncrypt.Internal.AcmeStates;

namespace Yaginx.YaginxAcmeLoaders
{
    internal class YaginxServerStartupState : SyncAcmeState
    {
        private readonly CertificateSelector _selector;
        private readonly ILogger<YaginxServerStartupState> _logger;

        public YaginxServerStartupState(
            AcmeStateMachineContext context,
            CertificateSelector selector,
            ILogger<YaginxServerStartupState> logger) :
            base(context)
        {
            _selector = selector;
            _logger = logger;
        }

        public override IAcmeState MoveNext()
        {
            if (_selector.HasCertForDomain(Context.CurrentDomain))
            {
                _logger.LogDebug("Certificate for {domainNames} already found.", Context.CurrentDomain);
                return MoveTo<YaginxCheckForRenewalState>();
            }

            return MoveTo<YaginxBeginCertificateCreationState>();
        }
    }    
}

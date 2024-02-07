using LettuceEncrypt.Internal;
using Certes.Acme.Resource;
using Certes.Acme;


namespace Yaginx.YaginxAcmeLoaders
{
    internal class YaginxTlsAlpn01DomainValidator : DomainOwnershipValidator
    {
        private readonly YaginxTlsAlpnChallengeResponder _tlsAlpnChallengeResponder;

        public YaginxTlsAlpn01DomainValidator(YaginxTlsAlpnChallengeResponder tlsAlpnChallengeResponder,
            IHostApplicationLifetime appLifetime,
            AcmeClient client, ILogger logger, string domainName) : base(appLifetime, client, logger, domainName)
        {
            _tlsAlpnChallengeResponder = tlsAlpnChallengeResponder;
        }

        public override async Task ValidateOwnershipAsync(IAuthorizationContext authzContext, CancellationToken cancellationToken)
        {
            try
            {
                await PrepareTlsAlpnChallengeResponseAsync(authzContext, _domainName, cancellationToken);
                await WaitForChallengeResultAsync(authzContext, cancellationToken);
            }
            finally
            {
                // cleanup after authorization is done to skip unnecessary cert lookup on all incoming SSL connections
                _tlsAlpnChallengeResponder.DiscardChallenge(_domainName);
            }
        }

        private async Task PrepareTlsAlpnChallengeResponseAsync(
            IAuthorizationContext authorizationContext,
            string domainName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tlsAlpnChallenge = await _client.CreateChallengeAsync(authorizationContext, ChallengeTypes.TlsAlpn01);

            _tlsAlpnChallengeResponder.PrepareChallengeCert(domainName, tlsAlpnChallenge.KeyAuthz);

            _logger.LogTrace("Waiting for server to start accepting HTTP requests");
            await _appStarted.Task;

            _logger.LogTrace("Requesting server to validate TLS/ALPN challenge");
            await _client.ValidateChallengeAsync(tlsAlpnChallenge);
        }
    }

}

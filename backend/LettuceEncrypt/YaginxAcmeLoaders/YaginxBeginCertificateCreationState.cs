using LettuceEncrypt;
using LettuceEncrypt.Internal;
using LettuceEncrypt.Internal.AcmeStates;

namespace Yaginx.YaginxAcmeLoaders
{
    internal class YaginxBeginCertificateCreationState : AcmeState
    {
        private readonly ILogger<YaginxServerStartupState> _logger;
        private readonly YaginxAcmeCertificateFactory _acmeCertificateFactory;
        private readonly CertificateSelector _selector;
        private readonly IEnumerable<ICertificateRepository> _certificateRepositories;

        public YaginxBeginCertificateCreationState(
            AcmeStateMachineContext context, ILogger<YaginxServerStartupState> logger,
            YaginxAcmeCertificateFactory acmeCertificateFactory,
            CertificateSelector selector, IEnumerable<ICertificateRepository> certificateRepositories)
            : base(context)
        {
            _logger = logger;
            _acmeCertificateFactory = acmeCertificateFactory;
            _selector = selector;
            _certificateRepositories = certificateRepositories;
        }

        public override async Task<IAcmeState> MoveNextAsync(CancellationToken cancellationToken)
        {
            var domainNames = new string[] { Context.CurrentDomain! };
            var accountEmailAddress = Context.EmailAddress!;
            try
            {
                var account = await _acmeCertificateFactory.GetOrCreateAccountAsync(accountEmailAddress, cancellationToken);
                _logger.LogInformation("Using account {accountId}", account.Id);

                _logger.LogInformation("Creating certificate for {hostname}",
                    string.Join(",", domainNames));

                var cert = await _acmeCertificateFactory.CreateCertificateAsync(domainNames, Context.KeyAlgorithm, Context.AdditionalIssuers, Context.AllowedChallengeTypes, cancellationToken);

                _logger.LogInformation("Created certificate {subjectName} ({thumbprint})",
                    cert.Subject,
                    cert.Thumbprint);

                await SaveCertificateAsync(cert, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Failed to automatically create a certificate for {hostname}", domainNames);
                throw;
            }

            return MoveTo<YaginxCheckForRenewalState>();
        }

        private async Task SaveCertificateAsync(X509Certificate2 cert, CancellationToken cancellationToken)
        {
            _selector.Add(cert);

            var saveTasks = new List<Task>();

            var errors = new List<Exception>();
            foreach (var repo in _certificateRepositories)
            {
                try
                {
                    saveTasks.Add(repo.SaveAsync(cert, cancellationToken));
                }
                catch (Exception ex)
                {
                    // synchronous saves may fail immediately
                    errors.Add(ex);
                }
            }

            await Task.WhenAll(saveTasks);

            if (errors.Count > 0)
            {
                throw new AggregateException("Failed to save cert to repositories", errors);
            }
        }
    }
}

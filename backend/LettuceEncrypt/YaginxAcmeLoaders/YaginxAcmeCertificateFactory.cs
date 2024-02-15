﻿using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using LettuceEncrypt.Accounts;
using LettuceEncrypt.Acme;
using LettuceEncrypt.Internal;
using LettuceEncrypt.Internal.PfxBuilder;
using KeyAlgorithm = LettuceEncrypt.KeyAlgorithm;

namespace Yaginx.YaginxAcmeLoaders
{
    internal class YaginxAcmeCertificateFactory
    {
        private readonly AcmeClientFactory _acmeClientFactory;
        private readonly TermsOfServiceChecker _tosChecker;
        private readonly IHttpChallengeResponseStore _challengeStore;
        private readonly IAccountStore _accountRepository;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;        
        private readonly IDnsChallengeProvider _dnsChallengeProvider;
        private readonly ICertificateAuthorityConfiguration _certificateAuthority;
        private readonly IPfxBuilderFactory _pfxBuilderFactory;
        private readonly TaskCompletionSource<object?> _appStarted = new();
        private AcmeClient? _client;
        private IKey? _acmeAccountKey;

        public YaginxAcmeCertificateFactory(
            AcmeClientFactory acmeClientFactory,
            TermsOfServiceChecker tosChecker,
            IHttpChallengeResponseStore challengeStore,
            ILogger<YaginxAcmeCertificateFactory> logger,
            IHostApplicationLifetime appLifetime,
            ICertificateAuthorityConfiguration certificateAuthority,
            IDnsChallengeProvider dnsChallengeProvider,
            IPfxBuilderFactory pfxBuilderFactory,
            IAccountStore? accountRepository = null)
        {
            _acmeClientFactory = acmeClientFactory;
            _tosChecker = tosChecker;
            _challengeStore = challengeStore;
            _logger = logger;
            _appLifetime = appLifetime;
            _dnsChallengeProvider = dnsChallengeProvider;
            _certificateAuthority = certificateAuthority;
            _pfxBuilderFactory = pfxBuilderFactory;

            appLifetime.ApplicationStarted.Register(() => _appStarted.TrySetResult(null));
            if (appLifetime.ApplicationStarted.IsCancellationRequested)
            {
                _appStarted.TrySetResult(null);
            }

            _accountRepository = accountRepository ?? new FileSystemAccountStore(logger, certificateAuthority);
        }

        public async Task<AccountModel> GetOrCreateAccountAsync(string emailAddress, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetAccountAsync(cancellationToken);

            _acmeAccountKey = account != null
                ? KeyFactory.FromDer(account.PrivateKey)
                : KeyFactory.NewKey(Certes.KeyAlgorithm.ES256);

            _client = _acmeClientFactory.Create(_acmeAccountKey);

            if (account != null && await ExistingAccountIsValidAsync())
            {
                return account;
            }

            return await CreateAccount(emailAddress, cancellationToken);
        }

        private async Task<AccountModel> CreateAccount(string emailAddress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_client == null || _acmeAccountKey == null)
            {
                throw new InvalidOperationException();
            }

            var tosUri = await _client.GetTermsOfServiceAsync();

            _tosChecker.EnsureTermsAreAccepted(tosUri);

            _logger.LogInformation("Creating new account for {email}", emailAddress);
            var accountId = await _client.CreateAccountAsync(emailAddress);

            var accountModel = new AccountModel
            {
                Id = accountId,
                EmailAddresses = new[] { emailAddress },
                PrivateKey = _acmeAccountKey.ToDer(),
            };

            await _accountRepository.SaveAccountAsync(accountModel, cancellationToken);

            return accountModel;
        }

        private async Task<bool> ExistingAccountIsValidAsync()
        {
            if (_client == null)
            {
                throw new InvalidOperationException();
            }

            // double checks the account is still valid
            Account existingAccount;
            try
            {
                existingAccount = await _client.GetAccountAsync();
            }
            catch (AcmeRequestException exception)
            {
                _logger.LogWarning(
                    "An account key was found, but could not be matched to a valid account. Validation error: {acmeError}",
                    exception.Error);
                return false;
            }

            if (existingAccount.Status != AccountStatus.Valid)
            {
                _logger.LogWarning(
                    "An account key was found, but the account is no longer valid. Account status: {status}." +
                    "A new account will be registered.",
                    existingAccount.Status);
                return false;
            }

            _logger.LogInformation("Using existing account for {contact}", existingAccount.Contact);

            if (existingAccount.TermsOfServiceAgreed != true)
            {
                var tosUri = await _client.GetTermsOfServiceAsync();
                _tosChecker.EnsureTermsAreAccepted(tosUri);
                await _client.AgreeToTermsOfServiceAsync();
            }

            return true;
        }

        public async Task<X509Certificate2> CreateCertificateAsync(string[] domainNames, KeyAlgorithm keyAlgorithm, string[] additionalIssuers, ChallengeType allowedChallengeTypes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_client == null)
            {
                throw new InvalidOperationException();
            }

            IOrderContext? orderContext = null;
            var orders = await _client.GetOrdersAsync();
            if (orders.Any())
            {
                var expectedDomains = new HashSet<string>(domainNames);
                foreach (var order in orders)
                {
                    var orderDetails = await _client.GetOrderDetailsAsync(order);
                    if (orderDetails.Status != OrderStatus.Pending)
                    {
                        continue;
                    }

                    var orderDomains = orderDetails
                        .Identifiers
                        .Where(i => i.Type == IdentifierType.Dns)
                        .Select(s => s.Value);

                    if (expectedDomains.SetEquals(orderDomains))
                    {
                        _logger.LogDebug("Found an existing order for a certificate");
                        orderContext = order;
                        break;
                    }
                }
            }

            if (orderContext == null)
            {
                _logger.LogDebug("Creating new order for a certificate");
                orderContext = await _client.CreateOrderAsync(domainNames);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var authorizations = await _client.GetOrderAuthorizations(orderContext);

            cancellationToken.ThrowIfCancellationRequested();
            await Task.WhenAll(BeginValidateAllAuthorizations(allowedChallengeTypes, authorizations, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
            return await CompleteCertificateRequestAsync(domainNames, keyAlgorithm, additionalIssuers, orderContext, cancellationToken);
        }

        private IEnumerable<Task> BeginValidateAllAuthorizations(ChallengeType allowedChallengeTypes, IEnumerable<IAuthorizationContext> authorizations,
            CancellationToken cancellationToken)
        {
            foreach (var authorization in authorizations)
            {
                yield return ValidateDomainOwnershipAsync(allowedChallengeTypes, authorization, cancellationToken);
            }
        }

        private async Task ValidateDomainOwnershipAsync(ChallengeType allowedChallengeTypes, IAuthorizationContext authorizationContext,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_client == null)
            {
                throw new InvalidOperationException();
            }

            var authorization = await _client.GetAuthorizationAsync(authorizationContext);
            var domainName = authorization.Identifier.Value;

            if (authorization.Status == AuthorizationStatus.Valid)
            {
                // Short circuit if authorization is already complete
                return;
            }

            _logger.LogDebug("Requesting authorization to create certificate for {domainName}", domainName);

            cancellationToken.ThrowIfCancellationRequested();

            var validators = new List<DomainOwnershipValidator>();

            //if (allowedChallengeTypes.HasFlag(ChallengeType.TlsAlpn01))
            //{
            //    validators.Add(new YaginxTlsAlpn01DomainValidator(
            //        _tlsAlpnChallengeResponder, _appLifetime, _client, _logger, domainName));
            //}

            if (allowedChallengeTypes.HasFlag(ChallengeType.Http01))
            {
                validators.Add(new YaginxHttp01DomainValidator(
                    _challengeStore, _appLifetime, _client, _logger, domainName));
            }

            //if (allowedChallengeTypes.HasFlag(ChallengeType.Dns01))
            //{
            //    validators.Add(new Dns01DomainValidator(
            //        _dnsChallengeProvider, _appLifetime, _client, _logger, domainName));
            //}

            if (validators.Count == 0)
            {
                var challengeTypes = string.Join(", ", Enum.GetNames(typeof(ChallengeType)));
                throw new InvalidOperationException(
                    "Could not find a method for validating domain ownership. " +
                    "Ensure at least one kind of these challenge types is configured: " + challengeTypes);
            }

            foreach (var validator in validators)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await validator.ValidateOwnershipAsync(authorizationContext, cancellationToken);
                    // The method above raises if validation fails. If no exception occurs, we assume validation completed successfully.
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Validation with {validatorType} failed with error: {error}",
                        validator.GetType().Name, ex.Message);
                }
            }

            throw new InvalidOperationException($"Failed to validate ownership of domainName '{domainName}'");
        }

        private async Task<X509Certificate2> CompleteCertificateRequestAsync(string[] domainNames, KeyAlgorithm keyAlgorithm, string[] additionalIssuers, IOrderContext order,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_client == null)
            {
                throw new InvalidOperationException();
            }

            var commonName = domainNames[0];
            _logger.LogDebug("Creating cert for {commonName}", commonName);

            var csrInfo = new CsrInfo
            {
                CommonName = commonName,
            };
            var privateKey = KeyFactory.NewKey((Certes.KeyAlgorithm)keyAlgorithm);
            var acmeCert = await _client.GetCertificateAsync(csrInfo, privateKey, order);

            _logger.LogAcmeAction("NewCertificate");

            var pfxBuilder = CreatePfxBuilder(additionalIssuers, acmeCert, privateKey);
            var pfx = pfxBuilder.Build("HTTPS Cert - " + domainNames, string.Empty);
            return new X509Certificate2(pfx, string.Empty, X509KeyStorageFlags.Exportable);
        }

        internal IPfxBuilder CreatePfxBuilder(string[] additionalIssuers, CertificateChain certificateChain, IKey certKey)
        {
            var pfxBuilder = _pfxBuilderFactory.FromChain(certificateChain, certKey);

            _logger.LogDebug(
                "Adding {IssuerCount} additional issuers to certes before building pfx certificate file",
                additionalIssuers.Length + _certificateAuthority.IssuerCertificates.Length);

            foreach (var issuer in additionalIssuers.Concat(_certificateAuthority.IssuerCertificates))
            {
                pfxBuilder.AddIssuer(Encoding.UTF8.GetBytes(issuer));
            }

            return pfxBuilder;
        }
    }
}

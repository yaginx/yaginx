using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class WebDomainRepository : YaginxCrudBaseRepository<WebDomain, WebDomainEntity>, IWebDomainRepository, ICertificateDomainRepsitory
    {
        public WebDomainRepository(IWoDbContextFactory factory, IMapper mapper, ILogger<WebDomainRepository> logger) : base(factory, mapper, logger)
        {
        }

        public Task<WebDomain> GetByNameAsync(string name)
        {
            return GetAsync<WebDomain>(x => x.Name == name);
        }

        public async Task UnFreeDomainAsync(string domain, string message)
        {
            var webDomain = await GetByNameAsync(domain);
            webDomain.IsUseFreeCert = false;
            webDomain.FreeCertMessage = message;
            await UpdateAsync(webDomain);
        }

        async Task<IEnumerable<string>> ICertificateDomainRepsitory.GetFreeCertDomainAsync()
        {
            var result = await GetByQueryAsync<WebDomain>(x => x.IsUseFreeCert);
            return result.Select(x => x.Name);
        }
    }
}

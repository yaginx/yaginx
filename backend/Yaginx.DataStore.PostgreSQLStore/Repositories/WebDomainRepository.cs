using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class WebDomainRepository : YaginxCrudBaseRepository<WebDomain, WebDomainEntity>, IWebDomainRepository
    {
        public WebDomainRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<WebDomainRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<WebDomain> GetByNameAsync(string name)
        {
            var entity = await FirstOrDefaultAsync<WebDomainEntity>(x => x.Name == name);
            return _mapper.Map<WebDomain>(entity);
        }

        public Task<IEnumerable<WebDomain>> SearchAsync()
        {
            return base.SearchAsync();
        }

        public Task<IEnumerable<WebDomain>> SearchAsync(bool useFreeCert = false)
        {
            return base.SearchAsync(x => x.IsUseFreeCert);
        }

        //public async Task UnFreeDomainAsync(string domain, string message)
        //{
        //    var webDomain = await GetByNameAsync(domain);
        //    webDomain.IsUseFreeCert = false;
        //    webDomain.FreeCertMessage = message;
        //    await UpdateAsync(webDomain);
        //}

        //async Task<IEnumerable<string>> ICertificateDomainRepsitory.GetFreeCertDomainAsync()
        //{
        //    var result = await GetByQueryAsync<WebDomain>(x => x.IsUseFreeCert);
        //    return result.Select(x => x.Name);
        //}
    }
}

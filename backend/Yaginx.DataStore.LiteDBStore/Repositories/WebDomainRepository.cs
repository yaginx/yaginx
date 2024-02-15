using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class WebDomainRepository : IWebDomainRepository, ICertificateDomainRepsitory
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;

        public WebDomainRepository(LiteDbDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task AddAsync(WebDomain webDomain)
        {
            await _databaseRepository.InsertAsync(webDomain);
        }

        public async Task DeleteAsync(long id)
        {
            await _databaseRepository.DeleteAsync<WebDomain>(id);
        }

        public Task<WebDomain> GetAsync(long id)
        {
            return _databaseRepository.GetAsync<WebDomain>(id);
        }

        public Task<WebDomain> GetByNameAsync(string name)
        {
            return _databaseRepository.GetAsync<WebDomain>(x => x.Name == name);
        }

        public async Task<IEnumerable<WebDomain>> SearchAsync()
        {
            return await _databaseRepository.SearchAsync<WebDomain>();
        }

        public async Task UpdateAsync(WebDomain webDomain)
        {
            await _databaseRepository.UpdateAsync(webDomain.Id, webDomain);
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
            var result = await _databaseRepository.SearchAsync<WebDomain>(x => x.IsUseFreeCert);
            return result.Select(x => x.Name);
        }
    }
}

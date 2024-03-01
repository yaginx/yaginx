using System.Linq.Expressions;
using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class WebDomainRepository : IWebDomainRepository
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

        public async Task<IEnumerable<WebDomain>> SearchAsync(Expression<Func<WebDomain, bool>> predicate = null)
        {
            return await _databaseRepository.SearchAsync<WebDomain>(predicate);
        }

        public async Task<IEnumerable<WebDomain>> SearchAsync(bool useFreeCert)
        {
            return await _databaseRepository.SearchAsync<WebDomain>(x => x.IsUseFreeCert);
        }

        public async Task UpdateAsync(WebDomain webDomain)
        {
            await _databaseRepository.UpdateAsync(webDomain.Id, webDomain);
        }
    }
}

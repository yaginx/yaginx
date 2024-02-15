using Yaginx.DomainModels;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class WebsiteRepository : IWebsiteRepository
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;

        public WebsiteRepository(LiteDbDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task AddAsync(Website website)
        {
            await _databaseRepository.InsertAsync(website);
        }

        public async Task DeleteAsync(long id)
        {
            await _databaseRepository.DeleteAsync<Website>(id);
        }

        public Task<Website> GetAsync(long id)
        {
            return _databaseRepository.GetAsync<Website>(id);
        }

        public async Task<Website> GetByNameAsync(string name)
        {
            return await _databaseRepository.GetAsync<Website>(x => x.Name == name);
        }

        public Task<IEnumerable<Website>> SearchAsync()
        {
            return _databaseRepository.SearchAsync<Website>();
        }

        public async Task UpdateAsync(Website website)
        {
            await _databaseRepository.UpdateAsync(website.Id, website);
        }
    }
}

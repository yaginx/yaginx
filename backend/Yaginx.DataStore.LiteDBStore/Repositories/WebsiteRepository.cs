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

        public void Add(Website website)
        {
            _databaseRepository.Insert(website);
        }

        public Website Get(long id)
        {
            return _databaseRepository.Get<Website>(id);
        }

        public Website GetByName(string name)
        {
            return _databaseRepository.Get<Website>(x => x.Name == name);
        }

        public List<Website> Search()
        {
            return _databaseRepository.Search<Website>().ToList();
        }

        public void Update(Website website)
        {
            _databaseRepository.Update(website.Id, website);
        }
    }
}

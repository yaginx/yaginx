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

        public void Add(WebDomain webDomain)
        {
            _databaseRepository.Insert(webDomain);
        }

        public WebDomain Get(long id)
        {
            return _databaseRepository.Get<WebDomain>(id);
        }

        public WebDomain GetByName(string name)
        {
            return _databaseRepository.Get<WebDomain>(x => x.Name == name);
        }

        public List<WebDomain> Search()
        {
            return _databaseRepository.Search<WebDomain>().ToList();
        }

        public void Update(WebDomain webDomain)
        {
            _databaseRepository.Update(webDomain);
        }

        IEnumerable<string> ICertificateDomainRepsitory.Search()
        {
            return Search().Select(x => x.Name);
        }
    }
}

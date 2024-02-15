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
            _databaseRepository.Update(webDomain.Id, webDomain);
        }

        public void UpdateDomainStatus(string domain, string message)
        {
            var webDomain = GetByName(domain);
            webDomain.IsUseFreeCert = false;
            webDomain.FreeCertMessage = message;
            Update(webDomain);
        }

        IEnumerable<string> ICertificateDomainRepsitory.GetFreeCertDomain()
        {
            return _databaseRepository.Search<WebDomain>(x => x.IsUseFreeCert).Select(x => x.Name);
        }
    }
}

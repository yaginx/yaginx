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
            _databaseRepository.Update(website);
        }
    }

    public class HostTrafficRepository : IHostTrafficRepository
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;

        public HostTrafficRepository(LiteDbDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }
        public List<HostTraffic> Search()
        {
            return _databaseRepository.Search<HostTraffic>().ToList();
        }

        public List<HostTraffic> Search(string hostName)
        {
            throw new NotImplementedException();
        }

        public void Upsert(HostTraffic hostTraffic)
        {
            var old = _databaseRepository.Get<HostTraffic>(x => x.HostName == hostTraffic.HostName);
            if (old == null)
                _databaseRepository.Insert(hostTraffic);
            else
            {
                old.InboundBytes += hostTraffic.InboundBytes;
                old.OutboundBytes += hostTraffic.OutboundBytes;
                old.RequestCounts += hostTraffic.RequestCounts;
                _databaseRepository.Update(hostTraffic);
            }
        }
    }
}

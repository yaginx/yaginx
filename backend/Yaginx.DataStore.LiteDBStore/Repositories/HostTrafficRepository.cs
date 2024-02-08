using Yaginx.DomainModels;

namespace Yaginx.DataStore.LiteDBStore.Repositories
{
    public class HostTrafficRepository : IHostTrafficRepository
    {
        private readonly LiteDbDatabaseRepository _databaseRepository;

        public HostTrafficRepository(LiteDbDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }
        public List<HostTraffic> Search()
        {
            return _databaseRepository.Search<HostTraffic>().OrderByDescending(x => x.Period).ThenBy(x => x.HostName).ToList();
        }

        public List<HostTraffic> Search(string hostName)
        {
            throw new NotImplementedException();
        }

        public void Upsert(HostTraffic hostTraffic)
        {
            var currentPeriod = TimePeriod.GetCurrentPeriod(SeqNoResetPeriod.Hourly, DateTime.UtcNow);

            var old = _databaseRepository.Get<HostTraffic>(x => x.HostName == hostTraffic.HostName && x.Period == currentPeriod.PeriodTs);
            if (old == null)
            {
                hostTraffic.Period = currentPeriod.PeriodTs;
                _databaseRepository.Insert(hostTraffic);
            }
            else
            {
                old.InboundBytes += hostTraffic.InboundBytes;
                old.OutboundBytes += hostTraffic.OutboundBytes;
                old.RequestCounts += hostTraffic.RequestCounts;
                _databaseRepository.Update(old);
            }
        }
    }
}

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
        public async Task<IEnumerable<HostTraffic>> SearchAsync()
        {
            var result = await _databaseRepository.SearchAsync<HostTraffic>();
            return result.OrderByDescending(x => x.Period).ThenBy(x => x.HostName).ToList();
        }

        public Task<IEnumerable<HostTraffic>> SearchAsync(string hostName)
        {
            throw new NotImplementedException();
        }

        public async Task UpsertAsync(HostTraffic hostTraffic)
        {
            var currentPeriod = TimePeriod.GetCurrentPeriod(SeqNoResetPeriod.Hourly, DateTime.UtcNow);

            var old = await _databaseRepository.GetAsync<HostTraffic>(x => x.HostName == hostTraffic.HostName && x.Period == currentPeriod.PeriodTs);
            if (old == null)
            {
                hostTraffic.Period = currentPeriod.PeriodTs;
                await _databaseRepository.InsertAsync(hostTraffic);
            }
            else
            {
                old.InboundBytes += hostTraffic.InboundBytes;
                old.OutboundBytes += hostTraffic.OutboundBytes;
                old.RequestCounts += hostTraffic.RequestCounts;
                await _databaseRepository.UpdateAsync(old.Id, old);
            }
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class HostTrafficRepository : YaginxCrudBaseRepository<HostTraffic, HostTrafficEntity>, IHostTrafficRepository
    {
        public HostTrafficRepository(IWoDbContextFactory factory, IMapper mapper, ILogger<HostTrafficRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<IEnumerable<HostTraffic>> SearchAsync(string hostName)
        {
            var query = await base.GetQueryAsync();
            var list = await query.Where(x => x.HostName == hostName).ToListAsync();
            return _mapper.Map<List<HostTraffic>>(list);
        }

        public async Task UpsertAsync(HostTraffic hostTraffic)
        {
            var currentPeriod = TimePeriod.GetCurrentPeriod(SeqNoResetPeriod.Hourly, DateTime.UtcNow);

            var old = await GetAsync<HostTraffic>(x => x.HostName == hostTraffic.HostName && x.Period == currentPeriod.PeriodTs);
            if (old == null)
            {
                hostTraffic.Period = currentPeriod.PeriodTs;
                await InsertAsync(hostTraffic);
            }
            else
            {
                old.InboundBytes += hostTraffic.InboundBytes;
                old.OutboundBytes += hostTraffic.OutboundBytes;
                old.RequestCounts += hostTraffic.RequestCounts;
                await UpdateAsync(old);
            }
        }
    }
}

using AgileLabs;
using AgileLabs.ComponentModels;
using AgileLabs.DynamicSearch;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{

    public class HostTrafficRepository : YaginxCrudBaseRepository<HostTraffic, HostTrafficEntity>, IHostTrafficRepository
    {
        public HostTrafficRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<HostTrafficRepository> logger) : base(factory, mapper, logger)
        {
        }

        public async Task<IEnumerable<HostTraffic>> SearchAsync(string hostName)
        {
            var query = await base.GetQueryAsync();
            var list = await query.Where(x => x.HostName == hostName).ToListAsync();
            return _mapper.Map<List<HostTraffic>>(list);
        }

        public async Task<Page<HostTraffic>> SearchAsync(SearchParameters searchParameters)
        {
            if (searchParameters.PageInfo.SortField.IsNullOrEmpty())
            {
                searchParameters.PageInfo.SortField = nameof(HostTrafficEntity.Period);
                searchParameters.PageInfo.SortDirection = "DESC";
            }
            var list = await GetByPageAsync<HostTrafficEntity>(searchParameters);
            return _mapper.Map<Page<HostTraffic>>(list);
        }

        public async Task UpsertAsync(HostTraffic hostTraffic)
        {
            var currentPeriod = TimePeriod.GetCurrentPeriod(SeqNoResetPeriod.Hourly, DateTime.UtcNow);

            var entity = await FirstOrDefaultAsync<HostTrafficEntity>(x => x.HostName == hostTraffic.HostName && x.Period == currentPeriod.PeriodTs);
            if (entity == null)
            {
                entity = _mapper.Map<HostTrafficEntity>(hostTraffic);
                entity.Period = currentPeriod.PeriodTs;
                await InsertAsync(entity);
            }
            else
            {
                entity.InboundBytes += hostTraffic.InboundBytes;
                entity.OutboundBytes += hostTraffic.OutboundBytes;
                entity.RequestCounts += hostTraffic.RequestCounts;
                await EntryUpdateAsync(entity);
            }
        }
    }
}

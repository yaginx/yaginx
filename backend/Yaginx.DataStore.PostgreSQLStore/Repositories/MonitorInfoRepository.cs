using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class MonitorInfoRepository : CrudRepository<ResourceMonitorInfoEntity>, IMonitorInfoRepository
    {
        private readonly IDbContextCommiter _dbContextCommiter;
        private readonly IMapper _mapper;

        public MonitorInfoRepository(IAgileLabDbContextFactory factory, IDbContextCommiter dbContextCommiter, IMapper mapper, ILogger<MonitorInfoRepository> logger)
            : base(factory, logger, mapper)
        {
            _dbContextCommiter = dbContextCommiter;
            _mapper = mapper;
        }

        public async Task AddAsync(ResourceMonitorInfo monitorInfoEntity)
        {
            var entity = _mapper.Map<ResourceMonitorInfoEntity>(monitorInfoEntity);
            await base.InsertAsync(entity);
            await _dbContextCommiter.CommitAsync();
        }

        public async Task<IList<ResourceMonitorInfo>> SearchAsync(DateTime beginTime, DateTime endTime)
        {
            var entityList = await GetByQueryAsync(x => x.Timestamp >= beginTime && x.Timestamp < endTime);
            return _mapper.Map<List<ResourceMonitorInfo>>(entityList);
        }
    }
}

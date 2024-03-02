using AutoMapper;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Abstracted.ContextFactories;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class MonitorInfoRepository : CrudRepository<ResourceMonitorInfoEntity>, IMonitorInfoRepository
    {
        private readonly IDbContextCommiter _dbContextCommiter;
        private readonly IMapper _mapper;

        public MonitorInfoRepository(IWoDbContextFactory factory, IDbContextCommiter dbContextCommiter, IMapper mapper, ILogger<MonitorInfoRepository> logger) : base(factory, logger)
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
    }
}

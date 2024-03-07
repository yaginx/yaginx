using AgileLabs;
using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class ResourceReportRepository : CrudRepository<ResourceReportEntity>, IResourceReportRepository
    {
        private readonly IMapper _mapper;

        public ResourceReportRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<ResourceReportRepository> logger) : base(factory, logger, mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<ResourceReportModel>> SearchAsync(ReportSearchRequest request)
        {
            var query = await GetQueryAsync();

            //var filterBuilder = Builders<ResourceReportEntity>.Filter;
            //var filter = filterBuilder.Empty;
            if (request.ResourceUuid.IsNotNullOrWhitespace())
            {
                //filter &= filterBuilder.Eq(x => x.ResourceUuid, request.ResourceUuid);
                query = query.Where(x => x.ResourceUuid == request.ResourceUuid);

            }
            //filter &= filterBuilder.Eq(x => x.CycleType, request.CycleType);
            //filter &= filterBuilder.Gte(x => x.ReportTime, request.BeginTime.FromEpochSeconds().ToLocalTime());
            //filter &= filterBuilder.Lte(x => x.ReportTime, request.EndTime.FromEpochSeconds().ToLocalTime());

            query = query.Where(x => x.CycleType == request.CycleType && x.ReportTime >= request.BeginTime.FromEpochSeconds() && x.ReportTime <= request.EndTime.FromEpochSeconds());

            //var resultList = await SearchAsync(filter);
            return _mapper.Map<List<ResourceReportModel>>(await query.ToListAsync());
        }
    }
}

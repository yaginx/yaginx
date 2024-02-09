using AgileLabs;
using AgileLabs.Storage.Mongo;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Yaginx.DataStore.MongoStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.MongoStore.Repositories
{
    internal class ResourceReportRepository : YaginxNoSqlBaseRepository<ResourceReportEntity>, IResourceReportRepository
    {
        private readonly IMapper _mapper;

        public ResourceReportRepository(MongodbContext<MongodbSetting> mongoDatabase, ILogger<ResourceReportRepository> logger, IMapper mapper) : base(mongoDatabase, logger)
        {
            _mapper = mapper;
        }

        public async Task<List<ResourceReportModel>> Search(ReportSearchRequest request)
        {
            var filterBuilder = Builders<ResourceReportEntity>.Filter;
            var filter = filterBuilder.Empty;
            if (request.ResourceUuid.IsNotNullOrWhitespace())
            {
                filter &= filterBuilder.Eq(x => x.ResourceUuid, request.ResourceUuid);
            }
            filter &= filterBuilder.Eq(x => x.CycleType, request.CycleType);
            filter &= filterBuilder.Gte(x => x.ReportTime, request.BeginTime.FromEpochSeconds().ToLocalTime());
            filter &= filterBuilder.Lte(x => x.ReportTime, request.EndTime.FromEpochSeconds().ToLocalTime());
            var resultList = await SearchAsync(filter);
            return _mapper.Map<List<ResourceReportModel>>(resultList);
        }
    }
}

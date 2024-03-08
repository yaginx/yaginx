using AgileLabs;
using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class ResourceReportRepository : CrudRepository<ResourceReportEntity>, IResourceReportRepository
    {
        private readonly IMapper _mapper;
        private readonly DapperBaseRepository _dapperBaseRepository;

        public ResourceReportRepository(IAgileLabDbContextFactory factory, IMapper mapper, ILogger<ResourceReportRepository> logger, DapperBaseRepository dapperBaseRepository) : base(factory, logger, mapper)
        {
            _mapper = mapper;
            _dapperBaseRepository = dapperBaseRepository;
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

        public async Task<List<ResourceReportModel>> SearchAsync(ReportCycleType cycleType, DateTime beginTime, DateTime endTime)
        {
            var beginUtcTime = beginTime.ToUniversalTime();
            var endUtcTime = endTime.ToUniversalTime();
            var entityList = await GetByQueryAsync(x => x.CycleType == cycleType && x.ReportTime >= beginUtcTime && x.ReportTime < endUtcTime);
            return _mapper.Map<List<ResourceReportModel>>(entityList);
        }

        public async Task UpsertAsync(ResourceReportModel resourceReport)
        {
            /*
                         //var update = Builders<ResourceReportEntity>.Update
            //    .SetOnInsert(x => x.ResourceUuid, resourceReport.ResourceUuid)
            //    .SetOnInsert(x => x.CycleType, resourceReport.CycleType)
            //    .SetOnInsert(x => x.ReportTime, resourceReport.ReportTime)
            //    .Set(x => x.RequestQty, resourceReport.RequestQty)
            //    .Set(x => x.Duration, resourceReport.Duration)
            //    .Set(x => x.Spider, resourceReport.Spider)
            //    .Set(x => x.Browser, resourceReport.Browser)
            //    .Set(x => x.Os, resourceReport.Os)
            //    .Set(x => x.StatusCode, resourceReport.StatusCode)
            //    .Set(x => x.CreateTime, resourceReport.CreateTime);
            //var result = await _reportRep.Collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            resource_uuid | cycle_type | report_time | request_qty | status_code | spider | browser | os | duration | create_time
             */
            var upsertSql = @"INSERT INTO resource_report (resource_uuid, cycle_type, report_time, request_qty, duration, spider, browser, os, status_code, create_time)
VALUES (@ResourceUuid,@CycleType,@ReportTime,@RequestQty,@Duration::jsonb,@Spider::jsonb,@Browser::jsonb,@Os::jsonb,@StatusCode::jsonb,@CreateTime)
ON CONFLICT(resource_uuid, cycle_type, report_time) 
DO UPDATE SET
  request_qty=@RequestQty,duration=@Duration::jsonb,spider=@Spider::jsonb, browser=@Browser::jsonb, os=@Os::jsonb, status_code=@StatusCode::jsonb, create_time=@CreateTime";

            var entity = _mapper.Map<ResourceReportEntity>(resourceReport);
            var insertModel = new
            {
                entity.ResourceUuid,
                entity.CycleType,
                entity.ReportTime,
                entity.RequestQty,
                entity.CreateTime,
                Duration = JsonConvert.SerializeObject(entity.Duration, Formatting.Indented),
                Spider = JsonConvert.SerializeObject(entity.Spider, Formatting.Indented),
                Browser = JsonConvert.SerializeObject(entity.Browser, Formatting.Indented),
                Os = JsonConvert.SerializeObject(entity.Os, Formatting.Indented),
                StatusCode = JsonConvert.SerializeObject(entity.StatusCode, Formatting.Indented)
            };
            await _dapperBaseRepository.ExecuteNoQueryAsync(upsertSql, insertModel);
        }
    }
}

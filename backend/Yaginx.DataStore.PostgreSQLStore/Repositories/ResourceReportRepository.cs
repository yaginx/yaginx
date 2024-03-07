using AgileLabs;
using AgileLabs.AspNet.WebApis.Exceptions;
using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.ConnectionStrings;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AgileLabs.Storage.PostgreSql;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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
            var entityList = await GetByQueryAsync(x => x.CycleType == cycleType && x.ReportTime >= beginTime && x.ReportTime < endTime);
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
VALUES (@ResourceUuid,@CycleType,@ReportTime,@RequestQty,@Duration,@Spider,@Browser,@Os,@StatusCode,@CreateTime)
ON CONFLICT(resource_uuid, cycle_type, report_time) 
DO UPDATE SET
  request_qty=@RequestQty,duration=@Duration,spider=@Spider, browser=@Browser, os=@Os, status_code=@StatusCode, create_time=@CreateTime";
            await _dapperBaseRepository.ExecuteNoQueryAsync(upsertSql, resourceReport);
        }
    }

    public class DapperBaseRepository : SqlBaseRepository
    {
        private readonly IDbDataSourceManager _dbConnectionManager;
        protected DapperBaseRepository(IDbDataSourceManager dbConnectionManager, IConnectionSafeHelper connectionSafeHelper, ILogger logger)
            : base(connectionSafeHelper, logger)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public override async Task<IDbConnection> GetDbConnectionAsync()
        {
            var dbDataSource = await _dbConnectionManager.GetDbDataSourceAsync();
            return dbDataSource.CreateConnection();
        }

        public virtual async Task<bool> IsTsVersionChanged(string tableName, string whereSql, object objParms, CancellationToken cancellation = default)
        {
            var sql = $"select 1 from {tableName} where {whereSql}";
            var unChanged = await ScalarAsync<bool>(sql, objParms, cancellationToken: cancellation);
            return !unChanged;
        }

        protected virtual async Task UpdateSignalEntityAsync(string sql, object obj, string tableName = "", string whereSql = "id = @id and ts=@ts", CancellationToken cancellation = default)
        {
            if (await IsTsVersionChanged(tableName, whereSql, obj))
            {
                throw new Exception($"表{tableName}检测到并发更新冲突");
            }

            var effectedRows = await ExecuteNoQueryAsync(sql, obj, cancellationToken: cancellation);
            if (effectedRows != 1)
            {
                throw new ApiException("", "更新失败");
            }
        }

        protected virtual async Task<Tuple<int, T1>> ExecuteAndQuery<T1>(string sql, object param = null, IDbTransaction outerTrans = null, CancellationToken cancellationToken = default)
        {
            WriteLog(sql, param);

            //GetConnection(outerTrans, out var conn, out var isTrans);
            var (conn, isTrans) = await GetConnectionAsync(outerTrans);

            try
            {
                await ConnectionSafeHelper.OpenConnectionAsync(conn, cancellationToken);

                if (!(conn is NpgsqlConnection pgConn))
                {
                    throw new RdbException($"{nameof(conn)}不是{nameof(NpgsqlConnection)}对象", null);
                }

                var commandDefinition = new CommandDefinition(sql, param, outerTrans, commandTimeout: 10, cancellationToken: cancellationToken);
                using var reader = await pgConn.ExecuteReaderAsync(commandDefinition);
                T1 returnValue = default;
                while (await reader.ReadAsync())
                {
                    returnValue = (T1)reader.GetValue(0);
                }
                var affectedRows = reader.RecordsAffected;
                return new Tuple<int, T1>(affectedRows, returnValue);
            }
            catch (NpgsqlException ex)
            {
                WriteLog(ex);
                throw;
            }
            finally
            {
                if (!isTrans)
                    await ConnectionSafeHelper.CloseConnectionAsync(conn);
            }
        }
    }
}

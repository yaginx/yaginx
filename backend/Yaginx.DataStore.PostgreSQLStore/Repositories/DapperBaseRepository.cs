using AgileLabs.AspNet.WebApis.Exceptions;
using AgileLabs.EfCore.PostgreSQL.ConnectionStrings;
using AgileLabs.Storage.PostgreSql;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class DapperBaseRepository : SqlBaseRepository
    {
        private readonly IDbDataSourceManager _dbConnectionManager;
        public DapperBaseRepository(IDbDataSourceManager dbConnectionManager, IConnectionSafeHelper connectionSafeHelper, ILogger<DapperBaseRepository> logger)
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

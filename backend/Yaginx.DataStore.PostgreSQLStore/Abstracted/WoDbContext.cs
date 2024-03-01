using AgileLabs;
using AgileLabs.Infrastructure;
using AgileLabs.WorkContexts.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public abstract class WoDbContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public WoDbContext(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WoDbContext>();
            _loggerFactory = loggerFactory;
        }

        //public IDbConnection Connection { get; set; }
        //public DbConnectionStringBuilder ConnectionStringBuilder { get; set; }
        //public InfraRdsType RdsType { get; set; }
        public DbDataSource DbDataSource { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (DbDataSource == null)
                {
                    throw new Exception($"{GetType().Name}未设置{nameof(DbDataSource)}属性");
                }

                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.EnableSensitiveDataLogging();

                switch (DbDataSource)
                {
                    case NpgsqlDataSource npgsqlDataSource:
                        optionsBuilder.UseNpgsql(npgsqlDataSource, b => b.MaxBatchSize(100)).UseSnakeCaseNamingConvention();
                        break;
                    default:
                        break;
                }

                // 如果是测试模式
                if (ServiceEnvironment.IsDebug() == true)
                {
                    // 使用控制台日志
                    optionsBuilder.UseLoggerFactory(_loggerFactory);
                    // 显示敏感数据
                    optionsBuilder.EnableSensitiveDataLogging(true);
                    // 详细查询异常
                    optionsBuilder.EnableDetailedErrors(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"数据库连接错误, 连接字符串信息:{DbDataSource.ConnectionString}, {ex.FullMessage()}");
                throw;
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var workContext = AgileLabContexts.Context.CurrentWorkContext;
            if (workContext == null)
            {
                throw new Exception($"{nameof(workContext)}为null，请检查上下文环境");
            }

            long? accountId = null;
            if (long.TryParse(workContext.Identity?.Id, out var currentAccountId))
            {
                accountId = currentAccountId;
            }

            foreach (var entry in ChangeTracker.Entries())
            {
                try
                {
                    //if (entry.Entity is AuditWithSoftDeleteEntity auditWithSoftDeleteEntity && entry.State == EntityState.Deleted)
                    //{
                    //    auditWithSoftDeleteEntity.IsDeleted = true;
                    //    auditWithSoftDeleteEntity.DeleteTime = DateTimeOffset.UtcNow;
                    //    auditWithSoftDeleteEntity.DeletedBy = accountId;
                    //    entry.State = EntityState.Modified;
                    //}

                    //if (entry.Entity is AuditEntity auditEntity)
                    //{
                    //    if (entry.State == EntityState.Added)
                    //    {
                    //        auditEntity.CreateTime = DateTimeOffset.UtcNow;
                    //        auditEntity.CreatedBy = accountId;
                    //        auditEntity.Ts = DateTime.Now.GetEpochSeconds();
                    //    }
                    //    else if (entry.State == EntityState.Modified)
                    //    {
                    //        auditEntity.UpdateTime = DateTimeOffset.UtcNow;
                    //        auditEntity.UpdatedBy = accountId;
                    //        entry.OriginalValues[nameof(auditEntity.Ts)] = auditEntity.Ts;
                    //        auditEntity.Ts = DateTime.Now.GetEpochSeconds();
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    var loggerFactory = workContext.Resolve<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger($"{GetType().FullName}.{nameof(SaveChangesAsync)}");
                    logger.LogError(ex, "DbContext SaveChange出现异常");
                }
            }
            try
            {
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                //foreach (var entry in ex.Entries)
                //{
                //    if (entry.Entity is Person)
                //    {
                //        var proposedValues = entry.CurrentValues;
                //        var databaseValues = entry.GetDatabaseValues();

                //        foreach (var property in proposedValues.Properties)
                //        {
                //            var proposedValue = proposedValues[property];
                //            var databaseValue = databaseValues[property];

                //            // TODO: decide which value should be written to database
                //            // proposedValues[property] = <value to be saved>;
                //        }

                //        // Refresh original values to bypass next concurrency check
                //        entry.OriginalValues.SetValues(databaseValues);
                //    }
                //    else
                //    {
                //        throw new NotSupportedException(
                //            "Don't know how to handle concurrency conflicts for "
                //            + entry.Metadata.Name);
                //    }
                //}
                throw;
            }
        }
    }
}

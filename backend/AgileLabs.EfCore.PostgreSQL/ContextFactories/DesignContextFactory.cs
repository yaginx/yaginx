using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AgileLabs.EfCore.PostgreSQL.ContextFactories
{
    public abstract class DesignContextFactoryAbstract : IDesignTimeDbContextFactory<CenterDbContext>
    {
        private readonly string _connectionString;

        public DesignContextFactoryAbstract(string connectionString)
        {
            _connectionString = connectionString;
        }
        public CenterDbContext CreateDbContext(string[] args)
        {
            var dbContext = new CenterDbContext(LoggerFactory.Create(builder => builder.AddConsole()));
            dbContext.DbDataSource = new NpgsqlDataSourceBuilder(_connectionString).Build();
            return dbContext;
        }
    }
}

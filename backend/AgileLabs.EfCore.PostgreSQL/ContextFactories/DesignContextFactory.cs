using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace AgileLabs.EfCore.PostgreSQL.ContextFactories
{
    public abstract class DesignContextFactoryAbstract<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : AgileLabDbContext
    {
        private readonly string _connectionString;

        public DesignContextFactoryAbstract(string connectionString)
        {
            _connectionString = connectionString;
        }
        public TDbContext CreateDbContext(string[] args)
        {
            var dbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), LoggerFactory.Create(builder => builder.AddConsole()));
            dbContext.DbDataSource = new NpgsqlDataSourceBuilder(_connectionString).Build();
            return dbContext;
        }
    }
}

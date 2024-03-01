using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public class DesignContextFactory : IDesignTimeDbContextFactory<CenterDbContext>
    {
        public CenterDbContext CreateDbContext(string[] args)
        {
            var npgsqlConnString = "Server=192.168.8.80;Port=5432;Database=yaginx;User Id=yaginx;Password=123456;Pooling=true;Application Name=Yaginx Apps;Include Error Detail=true;";

            var dbContext = new CenterDbContext(LoggerFactory.Create(builder => builder.AddConsole()));
            dbContext.DbDataSource = new NpgsqlDataSourceBuilder(npgsqlConnString).Build();
            return dbContext;
        }
    }
}

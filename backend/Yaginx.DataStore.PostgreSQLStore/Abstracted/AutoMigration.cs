using AgileLabs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yaginx.DataStore.PostgreSQLStore.Abstracted.ContextFactories;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    public class AutoMigration : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = AgileLabContexts.Context.CreateScopeWithWorkContext())
            {
                var services = scope.WorkContext.ServiceProvider;
                var dbContextFactory = services.GetRequiredService<IWoDbContextFactory>();
                var dbContext = await dbContextFactory.GetDefaultDbContextAsync();

                // 运行迁移
                await dbContext.Database.MigrateAsync(stoppingToken);
            }
        }
    }
}

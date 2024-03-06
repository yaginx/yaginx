using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using Microsoft.Extensions.Hosting;

namespace AgileLabs.EfCore.PostgreSQL
{
    public class AutoMigration : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = AgileLabContexts.Context.CreateScopeWithWorkContext())
            {
                var services = scope.WorkContext.ServiceProvider;
                var dbContextFactory = services.GetRequiredService<IAgileLabDbContextFactory>();
                var dbContext = await dbContextFactory.GetDefaultDbContextAsync();

                // 运行迁移
                await dbContext.Database.MigrateAsync(stoppingToken);
            }
        }
    }
}

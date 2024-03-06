using AgileLabs.EfCore.PostgreSQL.Commiters;
using AgileLabs.EfCore.PostgreSQL.ConnectionStrings;
using AgileLabs.EfCore.PostgreSQL.ContextFactories;
using AgileLabs.Storage.PostgreSql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AgileLabs.EfCore.PostgreSQL
{
    public static class ServcieREgisterExtension
    {
        public static void RegisterDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            /*
     有关DbContext的设计
    每个业务系统只能由一个默认的DbContext, CrudRepository<>直接使用默认的DbContext
    还可以有多个指定的DbContext, 但是指定的DbContext必须通过IWoDbContextFactory<TDbContext>及自己独立的Repository进行操作, 不可以使用CrudRepository
     */

            // 业务系统默认的DbContext
            // 下面分开注册的目的是:在同一个Scope确保拿到的DbContext是唯一的
            services.AddScoped<AgileLabDbContextFactory<TDbContext>>();
            services.AddScoped<IWoDbContextFactory<TDbContext>>(sp => sp.GetRequiredService<AgileLabDbContextFactory<TDbContext>>());
            services.AddScoped<IAgileLabDbContextFactory>(sp => sp.GetRequiredService<IWoDbContextFactory<TDbContext>>());
            services.AddScoped(sp => sp.GetRequiredService<AgileLabDbContextFactory<TDbContext>>().GetDbContextAsync().Result);

            services.AddSingleton<AutoCommiterMiddleware>();

            services.AddHostedService<AutoMigration>();
            services.AddScoped(typeof(CrudRepository<>));
            services.AddScoped(typeof(CrudRepository));

            services.TryAddScoped<IConnectionSafeHelper, ConnectionSafeHelper>();
            services.TryAddScoped<IDbContextCommiter, DbContextCommiter>();
            services.TryAddScoped<IDbDataSourceManager, DbDataSourceManager>();
        }

    }
}

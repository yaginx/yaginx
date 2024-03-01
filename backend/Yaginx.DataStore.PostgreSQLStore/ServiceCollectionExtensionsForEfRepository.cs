using AgileLabs.Storage.PostgreSql;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yaginx.DataStore.PostgreSQLStore.Abstracted;
using Yaginx.DataStore.PostgreSQLStore.Repositories;
using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Yaginx.DataStore.PostgreSQLStore
{
    public static class ServiceCollectionExtensionsForEfRepository
    {
        public static IServiceCollection AddedPostgreSQLStore(this IServiceCollection services, AppBuildContext buildContext)
        {
            //var configuration = buildContext.Configuration;
            //var connectonString = configuration.GetConnectionString("Default");
            //if (string.IsNullOrEmpty(connectonString))
            //{
            //    throw new InvalidOperationException($"Db {nameof(connectonString)} 不能为空");
            //}

            //services.AddDbContextPool<CenterDbContext>(options =>
            //{
            //    options.EnableSensitiveDataLogging();
            //    options.UseNpgsql(connectonString).UseSnakeCaseNamingConvention();
            //});

            services.RegisterDbContext<CenterDbContext>();
            services.AddHostedService<AutoMigration>();
            services.AddScoped(typeof(CrudRepository<>));
            services.AddScoped(typeof(CrudRepository));

            services.TryAddScoped<IConnectionSafeHelper, ConnectionSafeHelper>();
            services.TryAddScoped<IDbContextCommiter, DbContextCommiter>();
            services.TryAddScoped<IDbDataSourceManager, DbDataSourceManager>();
            services.AddSingleton<IStartupFilter, DefaultAutoCommiterStartupFilter>();

            services.AddScoped<IWebDomainRepository, WebDomainRepository>();
            services.AddScoped<IWebsiteRepository, WebsiteRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHostTrafficRepository, HostTrafficRepository>();
            services.AddScoped<ICertificateDomainRepsitory, WebDomainRepository>();

            return services;
        }

        public static void RegisterDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            /*
     有关DbContext的设计
    每个业务系统只能由一个默认的DbContext, CrudRepository<>直接使用默认的DbContext
    还可以有多个指定的DbContext, 但是指定的DbContext必须通过IWoDbContextFactory<TDbContext>及自己独立的Repository进行操作, 不可以使用CrudRepository
     */

            // 业务系统默认的DbContext
            // 下面分开注册的目的是:在同一个Scope确保拿到的DbContext是唯一的
            services.AddScoped<WoDbContextFactory<TDbContext>>();
            services.AddScoped<IWoDbContextFactory<TDbContext>>(sp => sp.GetRequiredService<WoDbContextFactory<TDbContext>>());
            services.AddScoped<IWoDbContextFactory>(sp => sp.GetRequiredService<WoDbContextFactory<TDbContext>>());
            services.AddScoped(sp => sp.GetRequiredService<WoDbContextFactory<TDbContext>>().GetDbContextAsync().Result);
        }
    }
}

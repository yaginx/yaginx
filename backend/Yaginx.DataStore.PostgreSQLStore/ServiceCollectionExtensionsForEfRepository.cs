using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.EfCore.PostgreSQL;
using AgileLabs.EfCore.PostgreSQL.Commiters;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yaginx.DataStore.PostgreSQLStore.Repositories;
using Yaginx.DomainModels;
using Yaginx.DomainModels.MonitorModels;

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
            services.AddScoped<DapperBaseRepository>();

            //services.AddSingleton<IStartupFilter, DefaultAutoCommiterStartupFilter>();

            services.AddScoped<IWebDomainRepository, WebDomainRepository>();
            services.AddScoped<IWebsiteRepository, WebsiteRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHostTrafficRepository, HostTrafficRepository>();

            // 注册仓储
            services.AddScoped<IMonitorInfoRepository, MonitorInfoRepository>();
            services.AddScoped<IResourceReportRepository, ResourceReportRepository>();

            return services;
        }
    }

    public class AutoCommiterPiplineRegister : IRequestPiplineRegister
    {
        public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
        {
            piplineActions.Register("AutoCommiter", RequestPiplineStage.BeginOfPipline, app => app.UseMiddleware<AutoCommiterMiddleware>());
            return piplineActions;
        }
    }
}

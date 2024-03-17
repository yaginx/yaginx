using AgileLabs;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Yaginx.SelfManagement.ApiDescriptions;
using Yaginx.SelfManagement.CustomEndpoints;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
using Yaginx.SelfManagement.Middlewares;

namespace Yaginx.SelfManagement
{
    public class ServiceCollectionExtensions : IServiceRegister
    {
        public int Order => 0;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            YaginxConsoleOption yaginxConsoleOption = new YaginxConsoleOption();
            buildContext.Configuration.GetSection("YaginxConsole").Bind(yaginxConsoleOption);
            services.AddSingleton(yaginxConsoleOption);

            #region Endpoints Register
            services.TryAddSingleton<ManagedApiConfigManager>();
            services.TryAddSingleton<ManagedApiEndpointFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ManagedApiMetadataMatchPolicy>());

            services.TryAddSingleton<IManagedApiDescription, DefaultManagedApiDescription>();
            #endregion
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddSingleton<ManagedApiProcessMiddleware>();
        }
    }
}

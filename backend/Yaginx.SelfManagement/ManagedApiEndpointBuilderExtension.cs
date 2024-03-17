using AgileLabs.AppRegisters;
using AgileLabs.AspNet;
using AgileLabs.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Yaginx.SelfManagement.CustomEndpoints;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
using Yaginx.SelfManagement.Middlewares;

namespace Yaginx.SelfManagement
{
    public static class ManagedApiEndpointBuilderExtension
    {
        /// <summary>
        /// Adds Reverse Proxy routes to the route table with the customized processing pipeline. The pipeline includes
        /// by default the initialization step and the final proxy step, but not LoadBalancingMiddleware or other intermediate components.
        /// </summary>
        public static ManagedApiConventionBuilder MapManagedApi(this IEndpointRouteBuilder endpoints)
        {
            if (endpoints is null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }
            var proxyAppBuilder = new ManagedApiApplicationBuilder(endpoints.CreateApplicationBuilder());
            //Init Middleware
            proxyAppBuilder.UseMiddleware<GlobalExceptionMiddleware>();
            proxyAppBuilder.UseMiddleware<ManagedApiInitializerMiddleware>();
            //proxyAppBuilder.UseMiddleware<AuthorizationMiddleware>();
            //proxyAppBuilder.UseMiddleware<ManagedApiRequestLogMiddleware>();
            //proxyAppBuilder.UseMiddleware<TraceInfoHeaderOutputMiddleware>();
            //proxyAppBuilder.UseMiddleware<RequestStatisticsMiddleware>();
            proxyAppBuilder.UseMiddleware<DiagnositicMiddleware>();
            proxyAppBuilder.MapDiagnositicRequest();

            // Final Process Middleware
            proxyAppBuilder.UseMiddleware<ManagedApiProcessMiddleware>();
            var app = proxyAppBuilder.Build();

            var proxyEndpointFactory = endpoints.ServiceProvider.GetRequiredService<ManagedApiEndpointFactory>();
            proxyEndpointFactory.SetProxyPipeline(app);
            return GetOrCreateDataSource(endpoints).DefaultBuilder;
        }

        private static ManagedApiConfigManager GetOrCreateDataSource(IEndpointRouteBuilder endpoints)
        {
            var dataSource = endpoints.DataSources.OfType<ManagedApiConfigManager>().FirstOrDefault();
            if (dataSource is null)
            {
                dataSource = endpoints.ServiceProvider.GetRequiredService<ManagedApiConfigManager>();
                endpoints.DataSources.Add(dataSource);

                // Config validation is async but startup is sync. We want this to block so that A) any validation errors can prevent
                // the app from starting, and B) so that all the config is ready before the server starts accepting requests.
                // Reloads will be async.
                dataSource.InitialLoadAsync().GetAwaiter().GetResult();
            }

            return dataSource;
        }
    }
}

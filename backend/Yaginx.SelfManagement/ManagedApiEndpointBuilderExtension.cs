using AgileLabs.AppRegisters;
using AgileLabs.AspNet;
using AgileLabs.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.RegularExpressions;
using Yaginx.SelfManagement.CustomEndpoints;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
using Yaginx.SelfManagement.Middlewares;
using Microsoft.AspNetCore.Builder;
using Scintillating.ProxyProtocol.Middleware;

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
            proxyAppBuilder.MapWhen(context => Regex.IsMatch(context.Request.Path, "/ip", RegexOptions.IgnoreCase),
                config => config.Run(async context =>
                {
                    var feature = context.Features.Get<IProxyProtocolFeature>();
                    if (feature != null)
                    {
                        context.Response.Headers["X-Connection-Orignal-Remote-EndPoint"] = feature.OriginalRemoteEndPoint?.ToString();
                        context.Response.Headers["X-Connection-Orignal-Local-EndPoint"] = feature.OriginalLocalEndPoint?.ToString();
                    }

                    context.Response.Headers["X-Request-Protocol"] = context.Request.Protocol;
                    context.Response.Headers["X-Connection-Remote-IP"] = context.Connection.RemoteIpAddress?.ToString();
                    context.Response.Headers["X-Connection-Remote-Port"] = context.Connection.RemotePort.ToString();
                    context.Response.Headers["X-Connection-Local-IP"] = context.Connection.LocalIpAddress?.ToString();
                    context.Response.Headers["X-Connection-Local-Port"] = context.Connection.LocalPort.ToString();

                    context.Response.Headers["X-Request-IsHttps"] = context.Request.IsHttps.ToString();
                    var rspObject = new { ProxyProtocol = feature?.ToString() };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(rspObject));
                }));

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

        static void MapGet(this IApplicationBuilder app, string pattern, RequestDelegate requestDelegate)
        {
            app.MapWhen(context => Regex.IsMatch(context.Request.Path, pattern, RegexOptions.IgnoreCase),
               config => config.Run(async context =>
               {
                   await requestDelegate(context);
               }));
        }
    }
}

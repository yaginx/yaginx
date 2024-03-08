using Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;
using Yaginx.SimpleProcessors.Middlewares;

namespace Yaginx.SimpleProcessors;
public static class SimpleProcessorBuilderExtension
{
    /// <summary>
    /// Adds Reverse Proxy routes to the route table using the default processing pipeline.
    /// </summary>
    public static SimpleProcessorConventionBuilder MapSimpleProcessor(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapSimpleProcessor(app =>
        {
            //app.UseSessionAffinity();
            //app.UseLoadBalancing();
            //app.UsePassiveHealthChecks();
        });
    }

    /// <summary>
    /// Adds Reverse Proxy routes to the route table with the customized processing pipeline. The pipeline includes
    /// by default the initialization step and the final proxy step, but not LoadBalancingMiddleware or other intermediate components.
    /// </summary>
    public static SimpleProcessorConventionBuilder MapSimpleProcessor(this IEndpointRouteBuilder endpoints, Action<ISimpleProcessorApplicationBuilder> configureApp)
    {
        if (endpoints is null)
        {
            throw new ArgumentNullException(nameof(endpoints));
        }
        if (configureApp is null)
        {
            throw new ArgumentNullException(nameof(configureApp));
        }
        {
            var proxyAppBuilder = new SimpleProcessorApplicationBuilder(endpoints.CreateApplicationBuilder());
            proxyAppBuilder.UseMiddleware<SimpleProcessorInitializerMiddleware>();
            configureApp(proxyAppBuilder);
            proxyAppBuilder.UseMiddleware<FeatureMiddleware>();
            var app = proxyAppBuilder.Build();

            var proxyEndpointFactory = endpoints.ServiceProvider.GetRequiredService<WebsitePreProcessEndpointFactory>();
            proxyEndpointFactory.SetProxyPipeline(app);
        }
        {
            var proxyAppBuilder = new SimpleProcessorApplicationBuilder(endpoints.CreateApplicationBuilder());
            proxyAppBuilder.UseMiddleware<SimpleProcessorInitializerMiddleware>();
            configureApp(proxyAppBuilder);
            proxyAppBuilder.UseMiddleware<AutoRedirectToHttpsMiddleware>();
            var app = proxyAppBuilder.Build();

            var proxyEndpointFactory = endpoints.ServiceProvider.GetRequiredService<Http2HttpsEndpointFactory>();
            proxyEndpointFactory.SetProxyPipeline(app);
        }
        return GetOrCreateDataSource(endpoints).DefaultBuilder;
    }

    private static SimpleProcessorConfigManager GetOrCreateDataSource(IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.DataSources.OfType<SimpleProcessorConfigManager>().FirstOrDefault();
        if (dataSource is null)
        {
            dataSource = endpoints.ServiceProvider.GetRequiredService<SimpleProcessorConfigManager>();
            endpoints.DataSources.Add(dataSource);

            // Config validation is async but startup is sync. We want this to block so that A) any validation errors can prevent
            // the app from starting, and B) so that all the config is ready before the server starts accepting requests.
            // Reloads will be async.
            dataSource.InitialLoadAsync().GetAwaiter().GetResult();
        }

        return dataSource;
    }
}

namespace Yaginx.SimpleProcessors;

public static class SimpleProcessorBuilder
{
    public static void ProxyBuilder(ISimpleProcessorApplicationBuilder app) => app.Use(ProxyForwarder);
    private static async Task ProxyForwarder(HttpContext context, Func<Task> next)
    {
        //var proxyFeature = context.GetReverseProxyFeature();
        //proxyFeature.AvailableDestinations = proxyFeature.AvailableDestinations;

        var serviceProvider = context.RequestServices;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Yaginx.ProxyForwarder");

        await next();

        //var errorFeature = context.GetForwarderErrorFeature();
        //if (errorFeature is not null)
        //{
        //    logger.LogError(0, errorFeature.Exception, $"转发异常-{errorFeature.Error}");
        //}
    }
}

using Microsoft.AspNetCore.Http.Extensions;
using Yaginx.SimpleProcessors.ConfigProviders;
using Yarp.ReverseProxy.Configuration;

namespace Yaginx.SimpleProcessors;
public interface ISimpleProcessorConfigChangeListener
{
    /// <summary>
    /// Invoked when an error occurs while loading the configuration.
    /// </summary>
    /// <param name="configProvider">The instance of the configuration provider that failed to provide the configuration.</param>
    /// <param name="exception">The thrown exception.</param>
    void ConfigurationLoadingFailed(ISimpleProcessorConfigProvider configProvider, Exception exception);

    /// <summary>
    /// Invoked once the configuration have been successfully loaded.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that have been loaded.</param>
    void ConfigurationLoaded(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs);

    /// <summary>
    /// Invoked when an error occurs while applying the configuration.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that were being processed.</param>
    /// <param name="exception">The thrown exception.</param>
    void ConfigurationApplyingFailed(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs, Exception exception);

    /// <summary>
    /// Invoked once the configuration has been successfully applied.
    /// </summary>
    /// <param name="proxyConfigs">The list of instances that have been applied.</param>
    void ConfigurationApplied(IReadOnlyList<ISimpleProcessorConfig> proxyConfigs);
}
internal sealed class AutoRedirectToHttpsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public AutoRedirectToHttpsMiddleware(RequestDelegate next, ILogger<AutoRedirectToHttpsMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task Invoke(HttpContext context)
    {
        //_ = context ?? throw new ArgumentNullException(nameof(context));

        //var config = context.GetRouteModel().Config;

        //if (config.MaxRequestBodySize.HasValue)
        //{
        //    var sizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        //    if (sizeFeature != null && !sizeFeature.IsReadOnly)
        //    {
        //        // -1 for disabled
        //        var limit = config.MaxRequestBodySize.Value;
        //        long? newValue = limit == -1 ? null : limit;
        //        sizeFeature.MaxRequestBodySize = newValue;
        //        Log.MaxRequestBodySizeSet(_logger, limit);
        //    }
        //}
        context.Response.Redirect($"https://{context.Request.Host}{context.Request.GetEncodedPathAndQuery()}");
        await Task.CompletedTask;
    }

    //private static class Log
    //{
    //    private static readonly Action<ILogger, long?, Exception?> _maxRequestBodySizeSet = LoggerMessage.Define<long?>(
    //        LogLevel.Debug,
    //        EventIds.MaxRequestBodySizeSet,
    //        "The MaxRequestBodySize has been set to '{limit}'.");

    //    public static void MaxRequestBodySizeSet(ILogger logger, long? limit)
    //    {
    //        _maxRequestBodySizeSet(logger, limit, null);
    //    }
    //}
}

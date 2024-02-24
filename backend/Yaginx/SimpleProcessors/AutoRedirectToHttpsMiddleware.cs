using Microsoft.AspNetCore.Http.Extensions;
using Yaginx.DomainModels;

namespace Yaginx.SimpleProcessors;
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

        var feature = context.Features.Get<ISimpleProcessorFeature>() ?? throw new InvalidOperationException($"{typeof(ISimpleProcessorFeature).FullName} is missing.");
        var model = feature.Model;
        if (model.Metadata.ContainsKey("RawModel"))
        {
            var webSite = (Website)model.Metadata["RawModel"];
        }
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
        context.Response.Redirect($"https://{model.PrimaryHost}{context.Request.GetEncodedPathAndQuery()}");
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

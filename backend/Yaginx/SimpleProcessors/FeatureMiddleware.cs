using Yaginx.DomainModels;

namespace Yaginx.SimpleProcessors;

public sealed class FeatureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public FeatureMiddleware(RequestDelegate next, ILogger<FeatureMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task Invoke(HttpContext context)
    {
        var feature = context.Features.Get<ISimpleProcessorFeature>() ?? throw new InvalidOperationException($"{typeof(ISimpleProcessorFeature).FullName} is missing.");
        var model = feature.Model;
        if (!model.Metadata.ContainsKey("RawModel"))
        {
            await context.Response.WriteAsync("OK");
        }
        var webSite = (Website)model.Metadata["RawModel"];
        var requestPath = context.Request.Path.Value.TrimStart('/');
        var preProcess = webSite.SimpleResponses.SingleOrDefault(x => x.Url.Equals(requestPath, StringComparison.OrdinalIgnoreCase));
        if (preProcess != null)
        {
            context.Response.StatusCode = preProcess.StatusCode;
            context.Response.ContentType = preProcess.ContentType;
            await context.Response.WriteAsync(preProcess.ResponseContent);
        }
        else
        {
            await context.Response.WriteAsync($"not found config for {context.Request.Path.Value}");
        }
    }
}

using Yaginx.SelfManagement;
using Yaginx.SelfManagement.Features;
namespace Yaginx.SelfManagement.Middlewares;
internal sealed class ManagedApiProcessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ManagedApiProcessMiddleware(RequestDelegate next, ILogger<ManagedApiProcessMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task Invoke(HttpContext context)
    {
        var feature = context.Features.Get<IManagedApiFeature>() ?? throw new InvalidOperationException($"{typeof(IManagedApiFeature).FullName} is missing.");
        var serviceType = feature.ServiceType;
        var methodInfo = feature.MethodInfo;

        if (methodInfo == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        try
        {
            var serviceInstance = context.RequestServices.GetService(serviceType) as IManagedApiService;
            serviceInstance.HttpContext = context;

            var mInvokerResult = methodInfo.Invoke(serviceInstance, new object[] { });
            if (mInvokerResult is Task task)
            {
                await task;
            }
            else if (mInvokerResult is Task<object> taskOfObject)
            {
                var result = await taskOfObject;
                if (result != null)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
            }
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(ex));
        }
    }
}


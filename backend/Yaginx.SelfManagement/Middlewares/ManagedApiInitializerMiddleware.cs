using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Yaginx.SelfManagement.ApiDescriptions;
using Yaginx.SelfManagement.CustomEndpoints;
using Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
using Yaginx.SelfManagement.Features;
namespace Yaginx.SelfManagement.Middlewares;
internal sealed class ManagedApiInitializerMiddleware
{
    private readonly ILogger _logger;
    private readonly YaginxConsoleOption _yaginxConsoleOption;
    private readonly RequestDelegate _next;

    public ManagedApiInitializerMiddleware(RequestDelegate next,
        ILogger<ManagedApiInitializerMiddleware> logger, YaginxConsoleOption yaginxConsoleOption)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _yaginxConsoleOption = yaginxConsoleOption;
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint()
           ?? throw new InvalidOperationException($"Routing Endpoint wasn't set for the current request.");

        var model = endpoint.Metadata.GetMetadata<ManagedApiMetadataModel>()
            ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(ManagedApiMetadataModel).FullName} metadata.");

        var pathBase = _yaginxConsoleOption.ConsolePath;

        context.Request.PathBase = pathBase;
        context.Request.Path = new PathString(context.Request.Path.Value.Substring(pathBase.Length));

        var apiDescription = context.RequestServices.GetRequiredService<IManagedApiDescription>();
        var apiUrl = context.GetRouteValue("catchall")?.ToString();
        //var apiMethodInfo = apiDescription.GetApiMethodInfo(model.ServiceType, apiUrl);

        //context.Features.Set<IManagedApiFeature>(new ManagedApiFeature
        //{
        //    ServiceType = model.ServiceType,
        //    MethodInfo = apiMethodInfo,
        //    Model = model,
        //});

        context.Features.Set<IManagedApiLogFeature>(new ManagedApiLogFeature()); 

        var activity = Observability.ManagedApiActivitySource.CreateActivity("ManagedApi.Handler", ActivityKind.Server);

        return activity is null
            ? _next(context)
            : AwaitWithActivity(context, activity);
    }

    private async Task AwaitWithActivity(HttpContext context, Activity activity)
    {
        context.SetManagedApiActivity(activity);

        activity.Start();
        try
        {
            await _next(context);
        }
        finally
        {
            activity.Dispose();
        }
    }
}


﻿using Microsoft.AspNetCore.Http.Timeouts;
using System.Diagnostics;
using Yaginx.DomainModels;
using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;
using Yarp.ReverseProxy.Model;
namespace Yaginx.SimpleProcessors.Middlewares;

internal sealed class SimpleProcessorInitializerMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public SimpleProcessorInitializerMiddleware(RequestDelegate next,
        ILogger<SimpleProcessorInitializerMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint()
           ?? throw new InvalidOperationException($"Routing Endpoint wasn't set for the current request.");

        var model = endpoint.Metadata.GetMetadata<WebsitePreProcessMetadataModel>()
            ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(WebsitePreProcessMetadataModel).FullName} metadata.");

        //var website = endpoint.Metadata.GetMetadata<Website>()
        //  ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(Website).FullName} metadata.");

        //var cluster = route.Cluster;
        //// TODO: Validate on load https://github.com/microsoft/reverse-proxy/issues/797
        //if (cluster is null)
        //{
        //    Log.NoClusterFound(_logger, route.Config.RouteId);
        //    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        //    return Task.CompletedTask;
        //}

#if NET8_0_OR_GREATER
        //// There's no way to detect the presence of the timeout middleware before this, only the options.
        //if (endpoint.Metadata.GetMetadata<RequestTimeoutAttribute>() != null
        //    && context.Features.Get<IHttpRequestTimeoutFeature>() == null
        //    // The feature is skipped if the request is already canceled. We'll handle canceled requests later for consistency.
        //    && !context.RequestAborted.IsCancellationRequested)
        //{
        //    Log.TimeoutNotApplied(_logger, route.Config.RouteId);
        //    // Out of an abundance of caution, refuse the request rather than allowing it to proceed without the configured timeout.
        //    throw new InvalidOperationException($"The timeout was not applied for route '{route.Config.RouteId}', ensure `IApplicationBuilder.UseRequestTimeouts()`"
        //        + " is called between `IApplicationBuilder.UseRouting()` and `IApplicationBuilder.UseEndpoints()`.");
        //}
#endif
        //var destinationsState = cluster.DestinationsState;
        context.Features.Set<ISimpleProcessorFeature>(new SimpleProcessorFeature
        {
            Model = model,
        });

        var activity = Observability.SimpleProcessorActivitySource.CreateActivity("proxy.forwarder", ActivityKind.Server);

        return activity is null
            ? _next(context)
            : AwaitWithActivity(context, activity);
    }

    private async Task AwaitWithActivity(HttpContext context, Activity activity)
    {
        context.SetYarpActivity(activity);

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

    private static class Log
    {
        private static readonly Action<ILogger, string, Exception> _noClusterFound = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.NoClusterFound,
            "Route '{routeId}' has no cluster information.");

        private static readonly Action<ILogger, string, Exception> _timeoutNotApplied = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.TimeoutNotApplied,
            "The timeout was not applied for route '{routeId}', ensure `IApplicationBuilder.UseRequestTimeouts()` is called between `IApplicationBuilder.UseRouting()` and `IApplicationBuilder.UseEndpoints()`.");

        public static void NoClusterFound(ILogger logger, string routeId)
        {
            _noClusterFound(logger, routeId, null);
        }

        public static void TimeoutNotApplied(ILogger logger, string routeId)
        {
            _timeoutNotApplied(logger, routeId, null);
        }
    }
}


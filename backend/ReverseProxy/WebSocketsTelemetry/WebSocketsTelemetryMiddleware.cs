// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Yarp.ReverseProxy.WebSocketsTelemetry;

internal sealed class WebSocketsTelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TimeProvider _timeProvider;

    public WebSocketsTelemetryMiddleware(RequestDelegate next, TimeProvider timeProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (WebSocketsTelemetry.Log.IsEnabled())
        {
            if (context.Features.Get<IHttpUpgradeFeature>() is { IsUpgradableRequest: true } upgradeFeature)
            {
                var upgradeWrapper = new HttpUpgradeFeatureWrapper(_timeProvider, context, upgradeFeature);
                return InvokeAsyncCore(upgradeWrapper, _next);
            }
#if NET7_0_OR_GREATER
            else if (context.Features.Get<IHttpExtendedConnectFeature>() is { IsExtendedConnect: true } connectFeature
                && string.Equals("websocket", connectFeature.Protocol, StringComparison.OrdinalIgnoreCase))
            {
                var connectWrapper = new HttpConnectFeatureWrapper(_timeProvider, context, connectFeature);
                return InvokeAsyncCore(connectWrapper, _next);
            }
#endif
        }

        return _next(context);
    }

    private static async Task InvokeAsyncCore(HttpUpgradeFeatureWrapper upgradeWrapper, RequestDelegate next)
    {
        upgradeWrapper.HttpContext.Features.Set<IHttpUpgradeFeature>(upgradeWrapper);

        try
        {
            await next(upgradeWrapper.HttpContext);
        }
        finally
        {
            if (upgradeWrapper.TelemetryStream is { } telemetryStream)
            {
                WebSocketsTelemetry.Log.WebSocketClosed(
                    telemetryStream.EstablishedTime.Ticks,
                    telemetryStream.GetCloseReason(upgradeWrapper.HttpContext),
                    telemetryStream.MessagesRead,
                    telemetryStream.MessagesWritten);
            }

            upgradeWrapper.HttpContext.Features.Set(upgradeWrapper.InnerUpgradeFeature);
        }
    }

#if NET7_0_OR_GREATER
    private static async Task InvokeAsyncCore(HttpConnectFeatureWrapper connectWrapper, RequestDelegate next)
    {
        connectWrapper.HttpContext.Features.Set<IHttpExtendedConnectFeature>(connectWrapper);

        try
        {
            await next(connectWrapper.HttpContext);
        }
        finally
        {
            if (connectWrapper.TelemetryStream is { } telemetryStream)
            {
                WebSocketsTelemetry.Log.WebSocketClosed(
                    telemetryStream.EstablishedTime.Ticks,
                    telemetryStream.GetCloseReason(connectWrapper.HttpContext),
                    telemetryStream.MessagesRead,
                    telemetryStream.MessagesWritten);
            }

            connectWrapper.HttpContext.Features.Set(connectWrapper.InnerConnectFeature);
        }
    }
#endif
}

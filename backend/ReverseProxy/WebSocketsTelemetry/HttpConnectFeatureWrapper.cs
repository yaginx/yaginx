// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if NET7_0_OR_GREATER

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Yarp.ReverseProxy.Utilities;

namespace Yarp.ReverseProxy.WebSocketsTelemetry;

internal sealed class HttpConnectFeatureWrapper : IHttpExtendedConnectFeature
{
    private readonly TimeProvider _timeProvider;

    public HttpContext HttpContext { get; private set; }

    public IHttpExtendedConnectFeature InnerConnectFeature { get; private set; }

    public WebSocketsTelemetryStream? TelemetryStream { get; private set; }

    public bool IsExtendedConnect => InnerConnectFeature.IsExtendedConnect;

    public string? Protocol => InnerConnectFeature.Protocol;

    public HttpConnectFeatureWrapper(TimeProvider timeProvider, HttpContext httpContext, IHttpExtendedConnectFeature connectFeature)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        InnerConnectFeature = connectFeature ?? throw new ArgumentNullException(nameof(connectFeature));
    }

    public async ValueTask<Stream> AcceptAsync()
    {
        Debug.Assert(TelemetryStream is null);
        var opaqueTransport = await InnerConnectFeature.AcceptAsync();
        TelemetryStream = new WebSocketsTelemetryStream(_timeProvider, opaqueTransport);
        return TelemetryStream;
    }
}

#endif

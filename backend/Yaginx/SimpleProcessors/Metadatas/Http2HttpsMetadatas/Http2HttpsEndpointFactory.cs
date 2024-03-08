using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;
using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;
using Yarp.ReverseProxy.Model;

namespace Yaginx.SimpleProcessors;

public sealed class Http2HttpsEndpointFactory
{
    private RequestDelegate? _pipeline;
    public Endpoint CreateHttp2HttpsEndpoint(string name, WebsitePreProcessMetadataModel model, int order = 0, IReadOnlyList<Action<EndpointBuilder>> conventions = null)
    {
        //var config = route.Config;
        //var match = config.Match;

        // Catch-all pattern when no path was specified
        var pathPattern = "/{**catchall}";// string.IsNullOrEmpty(match.Path) ? "/{**catchall}" : match.Path;

        var endpointBuilder = new RouteEndpointBuilder(
            requestDelegate: _pipeline ?? throw new InvalidOperationException("The pipeline hasn't been provided yet."),
            routePattern: RoutePatternFactory.Parse(pathPattern),
            order: order)
        {
            DisplayName = name
        };

        endpointBuilder.Metadata.Add(model);

        var relatedHosts = model.RelatedHost;
        var primaryHost = model.PrimaryHost;

        var matchers = new List<Http2HttpsMetadataMatcher>(relatedHosts?.Length ?? 0 + 1);
        matchers.Add(new Http2HttpsMetadataMatcher(primaryHost, relatedHosts));
        endpointBuilder.Metadata.Add(new Http2HttpsMetadata(matchers));

        for (var i = 0; i < conventions.Count; i++)
        {
            conventions[i](endpointBuilder);
        }

        return endpointBuilder.Build();
    }
    public void SetProxyPipeline(RequestDelegate pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }
}

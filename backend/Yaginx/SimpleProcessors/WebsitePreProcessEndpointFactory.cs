using Microsoft.AspNetCore.Routing.Patterns;
using System.Collections.Immutable;
using Yaginx.DomainModels;
using Yaginx.SimpleProcessors.ConfigProviders;
using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;
using Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

namespace Yaginx.SimpleProcessors;

public sealed class WebsitePreProcessEndpointFactory
{
    private RequestDelegate? _pipeline;
    public Endpoint CreateEndpoint(string name, WebsitePreProcessMetadataModel model, Website website, int order = 0, IReadOnlyList<Action<EndpointBuilder>> conventions = null)
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
        endpointBuilder.Metadata.Add(website);

        var preProcessUrl = website.SimpleResponses.Select(x => $"/{x.Url}").ToImmutableSortedSet();
        var relatedHosts = model.RelatedHost;
        var primaryHost = model.PrimaryHost;

        var matchers = new List<WebsitePreProcessMetadataMatcher>(relatedHosts?.Length ?? 0 + 1);
        matchers.Add(new WebsitePreProcessMetadataMatcher() { Host = primaryHost, Urls = preProcessUrl });
        if (relatedHosts != null)
        {
            foreach (var item in relatedHosts)
            {
                if (item.Equals(primaryHost, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                matchers.Add(new WebsitePreProcessMetadataMatcher() { Host = item, Urls = preProcessUrl });
            }
        }
        endpointBuilder.Metadata.Add(new WebsitePreProcessMetadata(matchers));

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
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;
using Yaginx.SimpleProcessors.RequestMetadatas;
using Yarp.ReverseProxy.Model;

namespace Yaginx.SimpleProcessors;

public sealed class SimpleProcessorEndpointFactory
{
    private RequestDelegate? _pipeline;
    public Endpoint CreateEndpoint(string name, string primaryHost, string[] releatedHosts, int order = 0, IReadOnlyList<Action<EndpointBuilder>> conventions = null)
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

        //endpointBuilder.Metadata.Add(route);

        //if (match.Hosts is not null && match.Hosts.Count != 0)
        //{
        //    endpointBuilder.Metadata.Add(new HostAttribute(match.Hosts.ToArray()));
        //}
        //Hosts: 8080.duke.feinian.net:* 
        //endpointBuilder.Metadata.Add(new HostAttribute("8080.duke.feinian.net"));
        //endpointBuilder.Metadata.Add(new Microsoft.AspNetCore.Routing.attri)

        /*
         //Host存在 并且Requst.Scheme是http
         //并且设置了自动转到Https
        Endpoint筛选条件: 设置了Host并且开启了自动从http转发到https
         */

        //var match = config.Match;
        //if (match.Headers is not null && match.Headers.Count > 0)
        //{
        //    var matchers = new List<RequestMetadataMatcher>(match.Headers.Count);
        //    matchers.Add(new RequestMetadataMatcher(header.Name, header.Values, header.Mode, header.IsCaseSensitive));
        //    endpointBuilder.Metadata.Add(new RequestMetadata(matchers));
        //}
        endpointBuilder.Metadata.Add(new RequestMetadata(new List<RequestMetadataMatcher>()));
        var matchers = new List<RequestMetadataMatcher>(releatedHosts.Length + 1);
        matchers.Add(new RequestMetadataMatcher(primaryHost));
        foreach (var item in releatedHosts)
        {
            matchers.Add(new RequestMetadataMatcher(item));
        }

        endpointBuilder.Metadata.Add(new RequestMetadata(matchers));
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

﻿using Microsoft.AspNetCore.Routing.Matching;

namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;
public class Http2HttpsMetadataMatchPolicy : MatcherPolicy, IEndpointComparerPolicy, IEndpointSelectorPolicy
{
    public IComparer<Endpoint> Comparer => new Http2HttpsMetadataEndpointComparer();

    public override int Order => 0;

    /// <inheritdoc/>
    bool IEndpointSelectorPolicy.AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        _ = endpoints ?? throw new ArgumentNullException(nameof(endpoints));

        // When the node contains dynamic endpoints we can't make any assumptions.
        if (ContainsDynamicEndpoints(endpoints))
        {
            return true;
        }

        return AppliesToEndpointsCore(endpoints);
    }
    private static bool AppliesToEndpointsCore(IReadOnlyList<Endpoint> endpoints)
    {
        return endpoints.Any(e =>
        {
            var metadata = e.Metadata.GetMetadata<IHttp2HttpsMetadata>();
            return metadata?.Matchers?.Length > 0;
        });
    }
    public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
    {
        _ = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        _ = candidates ?? throw new ArgumentNullException(nameof(candidates));

        var headers = httpContext.Request.Headers;

        for (var i = 0; i < candidates.Count; i++)
        {
            if (!candidates.IsValidCandidate(i))
            {
                continue;
            }

            var matchers = candidates[i].Endpoint.Metadata.GetMetadata<IHttp2HttpsMetadata>()?.Matchers;

            if (matchers is null)
            {
                continue;
            }

            foreach (var matcher in matchers)
            {
                //var headerExists = headers.TryGetValue(matcher.Name, out var requestHeaderValues);
                //var valueIsEmpty = StringValues.IsNullOrEmpty(requestHeaderValues);

                var host = httpContext.Request.Host.Host;

                var matched = host.Equals(matcher.PrimaryHost) && httpContext.Request.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase);

                if (!matched)
                {
                    matched = matcher.RelatedHost.Contains(host);
                }

                if (!matched)
                {
                    candidates.SetValidity(i, false);
                    break;
                }
            }
        }

        return Task.CompletedTask;
    }
}

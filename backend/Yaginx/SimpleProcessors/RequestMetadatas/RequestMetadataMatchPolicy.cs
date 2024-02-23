using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Primitives;

namespace Yaginx.SimpleProcessors.RequestMetadatas;
public class RequestMetadataMatchPolicy : MatcherPolicy, IEndpointComparerPolicy, IEndpointSelectorPolicy
{
    public IComparer<Endpoint> Comparer => new RequestMetadataEndpointComparer();

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
            var metadata = e.Metadata.GetMetadata<IRequestMetadata>();
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

            var matchers = candidates[i].Endpoint.Metadata.GetMetadata<IRequestMetadata>()?.Matchers;

            if (matchers is null)
            {
                continue;
            }

            foreach (var matcher in matchers)
            {
                //var headerExists = headers.TryGetValue(matcher.Name, out var requestHeaderValues);
                //var valueIsEmpty = StringValues.IsNullOrEmpty(requestHeaderValues);

                if (!httpContext.Request.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
                {
                    candidates.SetValidity(i, false);
                }

                //var matched = matcher.Mode switch
                //{
                //    HeaderMatchMode.Exists => !valueIsEmpty,
                //    HeaderMatchMode.NotExists => !headerExists,
                //    //HeaderMatchMode.ExactHeader => !valueIsEmpty && TryMatchExactOrPrefix(matcher, requestHeaderValues),
                //    //HeaderMatchMode.HeaderPrefix => !valueIsEmpty && TryMatchExactOrPrefix(matcher, requestHeaderValues),
                //    //HeaderMatchMode.Contains => !valueIsEmpty && TryMatchContainsOrNotContains(matcher, requestHeaderValues),
                //    //HeaderMatchMode.NotContains => valueIsEmpty || TryMatchContainsOrNotContains(matcher, requestHeaderValues),
                //    _ => false
                //};

                //if (!matched)
                //{
                //    candidates.SetValidity(i, false);
                //    break;
                //}
            }
        }

        return Task.CompletedTask;
    }
}

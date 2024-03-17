using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Yaginx.SelfManagement.ApiDescriptions;

namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;
public class ManagedApiMetadataMatchPolicy : MatcherPolicy, IEndpointComparerPolicy, IEndpointSelectorPolicy
{
    private readonly IManagedApiDescription _managedApiDescription;
    private readonly YaginxConsoleOption _yaginxConsoleOption;

    public IComparer<Endpoint> Comparer => new ManagedApiMetadataEndpointComparer();

    public override int Order => 0;

    public ManagedApiMetadataMatchPolicy(IManagedApiDescription managedApiDescription, YaginxConsoleOption yaginxConsoleOption)
    {
        _managedApiDescription = managedApiDescription;
        _yaginxConsoleOption = yaginxConsoleOption;
    }

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
            var metadata = e.Metadata.GetMetadata<IManagedApiUrlMetadata>();
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

            var matchers = candidates[i].Endpoint.Metadata.GetMetadata<IManagedApiUrlMetadata>()?.Matchers;

            if (matchers is null)
            {
                continue;
            }

            var matched = httpContext.Request.Path.Value.StartsWith(_yaginxConsoleOption.ConsolePath);

            //var routeValues = candidates[i].Values;

            //foreach (var matcher in matchers)
            //{
            //    routeValues.TryGetRouteValue<string>("brand", out var brand);
            //    routeValues.TryGetRouteValue<string>("group", out var group);
            //    routeValues.TryGetRouteValue<string>("service", out var service);

            //    var matched = string.Equals(brand, matcher.Brand, StringComparison.OrdinalIgnoreCase)
            //        && string.Equals(group, matcher.Group, StringComparison.OrdinalIgnoreCase)
            //        && string.Equals(service, matcher.ServiceName, StringComparison.OrdinalIgnoreCase);

            //    if (!matched)
            //    {
            //        candidates.SetValidity(i, false);
            //        break;
            //    }
            //}

            if (!matched)
            {
                candidates.SetValidity(i, false);
                break;
            }
        }

        return Task.CompletedTask;
    }
}

internal static class RouteValueExtension
{
    public static bool TryGetRouteValue<T>(this RouteValueDictionary dic, string name, out T outValue)
    {
        if (dic.TryGetValue(name, out var value) && value is T tValue)
        {
            outValue = tValue;
            return true;
        }

        outValue = default;
        return false;
    }
}

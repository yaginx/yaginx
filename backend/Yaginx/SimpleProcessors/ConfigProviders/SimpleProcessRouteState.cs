
namespace Yaginx.SimpleProcessors.ConfigProviders;

public class SimpleProcessRouteState
{
    public string RouteId { get; init; }
    public string PrimaryHost { get; init; }
    public string[] RelatedHost { get; init; }
    public IReadOnlyDictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// A cached Endpoint that will be cleared and rebuilt if the RouteConfig or cluster config change.
    /// </summary>
    internal Endpoint? CachedWebsitePreProcessEndpoint { get; set; }
    internal Endpoint? CachedHttp2HttpsEndpoint { get; set; }
}
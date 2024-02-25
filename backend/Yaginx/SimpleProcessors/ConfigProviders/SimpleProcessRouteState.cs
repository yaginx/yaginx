
namespace Yaginx.SimpleProcessors.ConfigProviders;

public class SimpleProcessRouteState
{
    public string RouteId { get; init; }
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }
    public IReadOnlyDictionary<string, object> Metadata { get; set; }

    /// <summary>
    /// A cached Endpoint that will be cleared and rebuilt if the RouteConfig or cluster config change.
    /// </summary>
    internal Endpoint? CachedWebsitePreProcessEndpoint { get; set; }
    internal Endpoint? CachedHttp2HttpsEndpoint { get; set; }
}
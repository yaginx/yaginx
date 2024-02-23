namespace Yaginx.SimpleProcessors.ConfigProviders;

public class SimpleProcessRouteState
{
    public string RouteId { get; set; }
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }

    /// <summary>
    /// A cached Endpoint that will be cleared and rebuilt if the RouteConfig or cluster config change.
    /// </summary>
    internal Endpoint? CachedEndpoint { get; set; }
}
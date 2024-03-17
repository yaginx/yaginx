namespace Yaginx.SelfManagement.CustomEndpoints
{
    public class ManagedApiRouteState
    {
        public string RouteId { get; init; }
        public IReadOnlyDictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// A cached Endpoint that will be cleared and rebuilt if the RouteConfig or cluster config change.
        /// </summary>
        internal Endpoint CachedEndpoint { get; set; }
    }
}

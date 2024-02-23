namespace Yaginx.SimpleProcessors;

internal static class EventIds
{
    public static readonly EventId LoadData = new EventId(1, "ApplyProxyConfig");
    public static readonly EventId ErrorSignalingChange = new EventId(2, "ApplyProxyConfigFailed");
    public static readonly EventId NoClusterFound = new EventId(4, "NoClusterFound");
    public static readonly EventId MaxRequestBodySizeSet = new EventId(60, "MaxRequestBodySizeSet");
    public static readonly EventId TimeoutNotApplied = new(64, nameof(TimeoutNotApplied));
}

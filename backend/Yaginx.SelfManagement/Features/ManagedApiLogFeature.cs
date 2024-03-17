namespace Yaginx.SelfManagement.Features;

public class ManagedApiLogFeature : IManagedApiLogFeature
{
    public Dictionary<string, string> Metadata { get; init; }

    public Dictionary<string, string> Tags { get; init; }

    public ManagedApiLogFeature()
    {
        Metadata = new Dictionary<string, string>();
        Tags = new Dictionary<string, string>();
    }
}

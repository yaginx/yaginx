namespace Yaginx.SelfManagement.Features;

public interface IManagedApiLogFeature
{
    Dictionary<string, string> Metadata { get; }
    Dictionary<string, string> Tags { get; }
}

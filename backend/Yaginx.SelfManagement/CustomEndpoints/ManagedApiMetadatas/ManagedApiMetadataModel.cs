namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

public class ManagedApiMetadataModel
{
    public string Brand { get; set; }
    public string Group { get; set; }
    public string ServiceName { get; set; }
    public Type ServiceType { get; set; }
    public IReadOnlyDictionary<string, object> Metadata { get; set; }
}

namespace Yaginx.SimpleProcessors.ConfigProviders;

public class RequestMetadataConfig
{
    public string RouteId { get; set; }
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }
}

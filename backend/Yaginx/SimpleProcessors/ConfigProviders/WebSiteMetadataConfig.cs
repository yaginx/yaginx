namespace Yaginx.SimpleProcessors.ConfigProviders;

public class WebSiteMetadataConfig
{
    public string RouteId { get; set; }
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }
    public IReadOnlyDictionary<string, object> Metadata { get; init; }
}

public class WebsitePreProcessMetadataModel
{
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }
    public IReadOnlyDictionary<string, object> Metadata { get; set; }
}

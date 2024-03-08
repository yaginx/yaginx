namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

public class WebsitePreProcessMetadataModel
{
    public string PrimaryHost { get; set; }
    public string[] RelatedHost { get; set; }
    public IReadOnlyDictionary<string, object> Metadata { get; set; }
}

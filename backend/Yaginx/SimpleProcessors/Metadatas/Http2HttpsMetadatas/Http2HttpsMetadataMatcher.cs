namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

internal sealed class Http2HttpsMetadataMatcher
{
    public Http2HttpsMetadataMatcher(string primaryHost, string[] relatedHost)
    {
        PrimaryHost = primaryHost;
        RelatedHost = relatedHost?.Where(x => !x.Equals(primaryHost)).ToArray();
    }

    public string PrimaryHost { get; }
    public string[] RelatedHost { get; set; }
}

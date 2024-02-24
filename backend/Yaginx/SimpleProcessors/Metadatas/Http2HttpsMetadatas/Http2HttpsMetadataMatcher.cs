namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

internal sealed class Http2HttpsMetadataMatcher
{
    public Http2HttpsMetadataMatcher(string host)
    {
        Host = host;
    }

    public string Host { get; }
}

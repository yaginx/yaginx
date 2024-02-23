namespace Yaginx.SimpleProcessors.RequestMetadatas;

internal sealed class RequestMetadataMatcher
{
    public RequestMetadataMatcher(string host)
    {
        Host = host;
    }

    public string Host { get; }
}

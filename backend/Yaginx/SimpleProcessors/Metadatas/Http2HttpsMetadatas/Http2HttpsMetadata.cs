namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

internal sealed class Http2HttpsMetadata : IHttp2HttpsMetadata
{
    public Http2HttpsMetadata(IReadOnlyList<Http2HttpsMetadataMatcher> matchers)
    {
        Matchers = matchers?.ToArray() ?? throw new ArgumentNullException(nameof(matchers));
    }

    /// <inheritdoc/>
    public Http2HttpsMetadataMatcher[] Matchers { get; }
}

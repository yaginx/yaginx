namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

internal interface IHttp2HttpsMetadata
{
    /// <summary>
    /// One or more matchers to apply to the request headers.
    /// </summary>
    Http2HttpsMetadataMatcher[] Matchers { get; }
}

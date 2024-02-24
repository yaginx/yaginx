using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

namespace Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

internal interface IWebsitePreProcessMetadata
{
    /// <summary>
    /// One or more matchers to apply to the request headers.
    /// </summary>
    WebsitePreProcessMetadataMatcher[] Matchers { get; }
}

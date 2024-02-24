using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

namespace Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

internal sealed class WebsitePreProcessMetadata : IWebsitePreProcessMetadata
{
    public WebsitePreProcessMetadata(IReadOnlyList<WebsitePreProcessMetadataMatcher> matchers)
    {
        Matchers = matchers?.ToArray() ?? throw new ArgumentNullException(nameof(matchers));
    }

    /// <inheritdoc/>
    public WebsitePreProcessMetadataMatcher[] Matchers { get; }
}
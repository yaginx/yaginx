using System.Collections.Immutable;
using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

namespace Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

internal sealed class WebsitePreProcessMetadataMatcher
{
    public string Host { get; init; }
    public ImmutableSortedSet<string> Urls { get; init; }
}

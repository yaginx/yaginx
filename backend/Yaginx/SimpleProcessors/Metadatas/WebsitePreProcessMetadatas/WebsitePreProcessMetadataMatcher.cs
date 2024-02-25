using System.Collections.Immutable;
using Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

namespace Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

internal sealed class WebsitePreProcessMetadataMatcher
{
    public string PrimaryHost { get; init; }
    public string[] RelatedHost { get; init; }
    public ImmutableSortedSet<string> Urls { get; init; }
}

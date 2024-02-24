using Microsoft.AspNetCore.Routing.Matching;
using Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

namespace Yaginx.SimpleProcessors.Metadatas.Http2HttpsMetadatas;

internal class Http2HttpsMetadataEndpointComparer : EndpointMetadataComparer<IHttp2HttpsMetadata>
{
    protected override int CompareMetadata(IHttp2HttpsMetadata x, IHttp2HttpsMetadata y)
    {
        return (y?.Matchers?.Length ?? 0).CompareTo(x?.Matchers?.Length ?? 0);
    }
}

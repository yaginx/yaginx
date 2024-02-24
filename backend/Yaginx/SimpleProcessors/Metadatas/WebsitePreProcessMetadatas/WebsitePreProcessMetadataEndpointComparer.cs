using Microsoft.AspNetCore.Routing.Matching;

namespace Yaginx.SimpleProcessors.Metadatas.WebsitePreProcessMetadatas;

internal class WebsitePreProcessMetadataEndpointComparer : EndpointMetadataComparer<IWebsitePreProcessMetadata>
{
    protected override int CompareMetadata(IWebsitePreProcessMetadata x, IWebsitePreProcessMetadata y)
    {
        return (y?.Matchers?.Length ?? 0).CompareTo(x?.Matchers?.Length ?? 0);
    }
}
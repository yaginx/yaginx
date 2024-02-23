using Microsoft.AspNetCore.Routing.Matching;

namespace Yaginx.SimpleProcessors.RequestMetadatas;

internal class RequestMetadataEndpointComparer : EndpointMetadataComparer<IRequestMetadata>
{
    protected override int CompareMetadata(IRequestMetadata? x, IRequestMetadata? y)
    {
        return (y?.Matchers?.Length ?? 0).CompareTo(x?.Matchers?.Length ?? 0);
    }
}

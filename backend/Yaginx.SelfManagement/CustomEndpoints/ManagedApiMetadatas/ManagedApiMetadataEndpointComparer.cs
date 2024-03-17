using Microsoft.AspNetCore.Routing.Matching;

namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

internal class ManagedApiMetadataEndpointComparer : EndpointMetadataComparer<IManagedApiUrlMetadata>
{
    protected override int CompareMetadata(IManagedApiUrlMetadata x, IManagedApiUrlMetadata y)
    {
        return (y?.Matchers?.Length ?? 0).CompareTo(x?.Matchers?.Length ?? 0);
    }
}

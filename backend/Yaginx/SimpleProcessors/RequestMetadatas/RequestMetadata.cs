namespace Yaginx.SimpleProcessors.RequestMetadatas;

internal sealed class RequestMetadata : IRequestMetadata
{
    public RequestMetadata(IReadOnlyList<RequestMetadataMatcher> matchers)
    {
        Matchers = matchers?.ToArray() ?? throw new ArgumentNullException(nameof(matchers));
    }

    /// <inheritdoc/>
    public RequestMetadataMatcher[] Matchers { get; }
}

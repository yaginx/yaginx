namespace Yaginx.SimpleProcessors.RequestMetadatas;

internal interface IRequestMetadata
{
    /// <summary>
    /// One or more matchers to apply to the request headers.
    /// </summary>
    RequestMetadataMatcher[] Matchers { get; }
}

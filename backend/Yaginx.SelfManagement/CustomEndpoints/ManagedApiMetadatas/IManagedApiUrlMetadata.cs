namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

internal interface IManagedApiUrlMetadata
{
    /// <summary>
    /// One or more matchers to apply to the request headers.
    /// </summary>
    ManagedApiUrlRuleMetadataMatcher[] Matchers { get; }
}

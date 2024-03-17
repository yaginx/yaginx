namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

internal sealed class ManagedApiUrlRuleMetadata : IManagedApiUrlMetadata
{
    public ManagedApiUrlRuleMetadata(IReadOnlyList<ManagedApiUrlRuleMetadataMatcher> matchers)
    {
        Matchers = matchers?.ToArray() ?? throw new ArgumentNullException(nameof(matchers));
    }

    /// <inheritdoc/>
    public ManagedApiUrlRuleMetadataMatcher[] Matchers { get; }
}

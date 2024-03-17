namespace Yaginx.SelfManagement.CustomEndpoints.ManagedApiMetadatas;

internal sealed class ManagedApiUrlRuleMetadataMatcher
{
    public ManagedApiUrlRuleMetadataMatcher(string brand, string group, Type serviceType, string serviceName)
    {
        Brand = brand;
        Group = group;
        ServiceType = serviceType;
        ServiceName = serviceName;
    }

    public string Brand { get; }
    public string Group { get; }
    public string ServiceName { get; set; }
    public Type ServiceType { get; }
}

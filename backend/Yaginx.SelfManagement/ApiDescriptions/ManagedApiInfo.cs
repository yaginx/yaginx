using System.Reflection;

namespace Yaginx.SelfManagement.ApiDescriptions;

public class ManagedApiInfo
{
    public string Brand { get; set; }
    public string Group { get; set; }
    public string ApiName { get; set; }
    public string ApiRoutePath { get; set; }
    public string ServiceName { get; set; }
    public Type ServiceType { get; set; }
    public MethodInfo ServiceMethod { get; set; }
}
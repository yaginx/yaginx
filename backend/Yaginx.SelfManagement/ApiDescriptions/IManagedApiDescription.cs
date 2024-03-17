using System.Reflection;

namespace Yaginx.SelfManagement.ApiDescriptions;

/// <summary>
/// 负责ManagedApi的元数据描述
/// </summary>
public interface IManagedApiDescription
{
    MethodInfo GetApiMethodInfo(Type serviceType, string url);
    List<ManagedApiInfo> GetManagedApis(Type serviceType);
    List<ManagedApiServiceInfo> GetManagedApiServiceList();
}

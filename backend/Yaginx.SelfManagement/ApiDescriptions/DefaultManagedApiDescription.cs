using AgileLabs.TypeFinders;
using System.Reflection;
using Yaginx.SelfManagement;
using Yaginx.SelfManagement.Attributes;

namespace Yaginx.SelfManagement.ApiDescriptions;

public class DefaultManagedApiDescription : IManagedApiDescription
{
    private readonly ITypeFinder _typeFinder;
    private IReadOnlyDictionary<Type, List<ManagedApiInfo>> _apiMethods;
    private readonly List<ManagedApiServiceInfo> _managedApiServices;

    public DefaultManagedApiDescription(ITypeFinder typeFinder)
    {
        _typeFinder = typeFinder;
        _managedApiServices = GetManagedApiServiceList();

        var managedApiInfos = new Dictionary<Type, List<ManagedApiInfo>>();
        foreach (var api in _managedApiServices)
        {
            var apis = GetManagedApis(api.ServiceType);
            managedApiInfos.Add(api.ServiceType, apis);
        }
        Interlocked.Exchange(ref _apiMethods, managedApiInfos);
    }

    public List<ManagedApiServiceInfo> GetManagedApiServiceList()
    {
        var managedApiInfos = new List<ManagedApiServiceInfo>();
        var managedApiServices = _typeFinder.FindClassesOfType<IManagedApiService>();
        foreach (var apiServiceType in managedApiServices)
        {
            var serviceAttribute = apiServiceType.GetCustomAttribute<ManagedApiServiceAttribute>(true);
            if (serviceAttribute == null)
                continue;

            managedApiInfos.Add(new ManagedApiServiceInfo
            {
                Brand = serviceAttribute.Brand,
                Group = serviceAttribute.Group,
                ServiceName = serviceAttribute.ServiceName ?? GetServiceName(apiServiceType),
                ServiceType = apiServiceType

            });
        }
        return managedApiInfos;
    }
    public List<ManagedApiInfo> GetManagedApis(Type apiServiceType)
    {
        var managedApiInfos = new List<ManagedApiInfo>();
        var serviceAttribute = apiServiceType.GetCustomAttribute<ManagedApiServiceAttribute>(true);
        if (serviceAttribute == null)
            return managedApiInfos;

        var apiMethods = apiServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (var apiMethodInfo in apiMethods)
        {
            var apiInfoAttribute = apiMethodInfo.GetCustomAttribute<ManagedApiAttribute>(true);
            if (apiInfoAttribute != null)
            {
                managedApiInfos.Add(new ManagedApiInfo
                {
                    Brand = serviceAttribute.Brand,
                    Group = serviceAttribute.Group,
                    ServiceName = serviceAttribute.ServiceName ?? GetServiceName(apiServiceType),
                    ServiceType = apiServiceType,
                    ServiceMethod = apiMethodInfo,
                    ApiName = apiMethodInfo.Name,
                    ApiRoutePath = apiInfoAttribute.Url
                });
            }
        }
        return managedApiInfos;
    }

    public MethodInfo GetApiMethodInfo(Type serviceType, string url)
    {
        if (!_apiMethods.ContainsKey(serviceType))
            return null;

        var serviceMethods = _apiMethods[serviceType];
        return serviceMethods.SingleOrDefault(x => x.ApiRoutePath.Equals(url, StringComparison.OrdinalIgnoreCase))?.ServiceMethod;
    }

    private string GetServiceName(Type serviceType)
    {
        var serviceName = serviceType.Name;
        if (serviceName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
        {
            return serviceName.Substring(0, serviceName.Length - 7);
        }
        return serviceName;
    }
}

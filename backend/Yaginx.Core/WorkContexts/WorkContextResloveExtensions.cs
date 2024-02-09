using AgileLabs.WorkContexts.Extensions;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Yaginx.WorkContexts;

/// <summary>
/// IWorkContext提供服务解析的相关扩展方法
/// </summary>
public static class WorkContextCoreResloveExtensions
{
    public static object ResolveUnregistered(this IWorkContext workContext, Type type)
    {
        if (workContext == null)
            throw new ArgumentNullException(nameof(workContext));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        Exception innerException = null;
        foreach (var constructor in type.GetConstructors())
        {
            try
            {
                //try to resolve constructor parameters
                var parameters = constructor.GetParameters().Select(parameter =>
                {
                    var service = workContext.Resolve(parameter.ParameterType);
                    if (service == null)
                        throw new Exception("Unknown dependency");
                    return service;
                });

                //all is ok, so create instance
                return Activator.CreateInstance(type, parameters.ToArray());
            }
            catch (Exception ex)
            {
                innerException = ex;
            }
        }

        throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
    }
    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="scope">Scope</param>
    /// <typeparam name="T">Type of resolved service</typeparam>
    /// <returns>Resolved service</returns>
    public static T Resolve<T>(this IWorkContext workContext) where T : class
    {
        if (workContext == null)
            throw new ArgumentNullException(nameof(workContext));

        var resolvedObject = workContext.Resolve(typeof(T));
        return resolvedObject != null && resolvedObject is T ? (T)resolvedObject : null;
    }

    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="type">Type of resolved service</param>
    /// <param name="scope">Scope</param>
    /// <returns>Resolved service</returns>
    public static object Resolve(this IWorkContext workContext, Type type)
    {
        if (workContext is null)
        {
            throw new ArgumentNullException(nameof(workContext));
        }

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        return workContext.ServiceProvider?.GetService(type);
    }

    public static TServaice ResolveByName<TServaice>(this IWorkContext workContext, string serviceName) where TServaice : class
    {
        if (workContext is null)
        {
            throw new ArgumentNullException(nameof(workContext));
        }

        if (serviceName == null)
            throw new ArgumentNullException(nameof(serviceName));

        return workContext.ServiceProvider?.GetAdvancedServiceProvider()?.LifetimeScope?.ResolveNamed<TServaice>(serviceName) ?? null;
    }

    public static TServaice ResolveByKey<TServaice>(this IWorkContext workContext, object key) where TServaice : class
    {
        if (workContext is null)
        {
            throw new ArgumentNullException(nameof(workContext));
        }

        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return workContext.ServiceProvider?.GetAdvancedServiceProvider()?.LifetimeScope?.ResolveKeyed<TServaice>(key);
    }
    public static object ResolveByName(this IWorkContext workContext, Type type, string serviceName)
    {
        if (workContext is null)
        {
            throw new ArgumentNullException(nameof(workContext));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (string.IsNullOrEmpty(serviceName))
        {
            throw new ArgumentException($"“{nameof(serviceName)}”不能为 null 或空。", nameof(serviceName));
        }

        return workContext.ServiceProvider?.GetAdvancedServiceProvider()?.LifetimeScope?.ResolveNamed(serviceName, type);
    }

    public static IEnumerable<TService> ResolveAll<TService>(this IWorkContext workContext) where TService : class
    {
        if (workContext is null)
        {
            throw new ArgumentNullException(nameof(workContext));
        }

        return workContext.ServiceProvider?.GetService<IEnumerable<TService>>();
    }
}

using AgileLabs.WebApp.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yaginx.DataStore.LiteDBStore;
using Yaginx.DataStore.LiteDBStore.Repositories;
using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Microsoft.AspNetCore.Hosting;

/// <summary>
/// Methods for configuring Kestrel.
/// </summary>
public static class LiteDBRegisterExtensions
{
    public static IServiceCollection UseLiteDBDataStore(this IServiceCollection services, AppBuildContext appBuildContext)
    {
        services.TryAddSingleton(sp =>
        {
            var connectionString = appBuildContext.Configuration.GetConnectionString("Default");
            if (connectionString == null)
                throw new Exception("未配置数据库连接字符串");
            var dbParms = connectionString.Split(';');

            var newDbParms = new List<string>();
            foreach (var item in dbParms)
            {
                var itemParms = item.Split('=');
                switch (itemParms[0].ToLower())
                {
                    case "filename":
                        newDbParms.Add(string.Join('=', itemParms[0], AppData.GetPath(itemParms[1])));
                        break;
                    default:
                        newDbParms.Add(item);
                        break;
                }
            }

            return new LiteDbDatabaseRepository(string.Join(';', newDbParms.ToArray()));
        });

        services.AddScoped<IWebDomainRepository, WebDomainRepository>();
        services.AddScoped<IWebsiteRepository, WebsiteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();


        services.AddScoped<ICertificateDomainRepsitory, WebDomainRepository>();
        return services;
    }
}
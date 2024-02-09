using AgileLabs.Storage.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.MongoStore;

public static class MongoServiceCoolectionExtensions
{
    public static void RegisterMongo(this IServiceCollection services, Action<MongodbSetting> optionsAction = null)
    {
        //var montodbSetting = new MongodbSetting();
        //configuration.GetSection("Mongo:Default")
        services.Configure(optionsAction);
        services.AddSingleton<MongodbContext<MongodbSetting>>();
        services.AddScoped(typeof(IAppNoSqlBaseRepository<>), typeof(YaginxNoSqlBaseRepository<>));

        // 注册仓储
        services.AddScoped<IMonitorInfoRepository, MonitorInfoRepository>();
    }
}

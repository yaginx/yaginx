using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Yaginx.MemoryBuses;

public static class BusServiceCollectionExtension
{
    public static void AddMemoryBus(this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryBus, MemoryBus>();        
        services.TryAddSingleton<IEventHandlerMappingCacheManager, EventHandlerMappingCacheManager>();

        services.AddScoped<IEventConsumer, MemoryEventConsumer>();// 每次都是新的实例来消费
    }
}

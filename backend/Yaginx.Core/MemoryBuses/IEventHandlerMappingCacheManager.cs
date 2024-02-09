namespace Yaginx.MemoryBuses;

public interface IEventHandlerMappingCacheManager
{
    List<Type> GetEventSubscriberTypeList(string commandTypeKey);
    Type GetEventType(string commandTypeKey);
}

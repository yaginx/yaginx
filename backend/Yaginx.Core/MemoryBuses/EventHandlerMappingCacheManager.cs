using AgileLabs.TypeFinders;
using System.Collections.Concurrent;
using Yaginx.MemoryBuses.Events;

namespace Yaginx.MemoryBuses;

/// <summary>
/// EventHandlerMappingManager
/// </summary>
public class EventHandlerMappingCacheManager : IEventHandlerMappingCacheManager
{
    readonly ConcurrentDictionary<string, EventHandlerMappingItem> _eventHandlerMappingCache = new ConcurrentDictionary<string, EventHandlerMappingItem>();

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="typeFinder"></param>
    public EventHandlerMappingCacheManager(ITypeFinder typeFinder)
    {
        var domainEventTypes = typeFinder.FindClassesOfType<IEvent>();
        foreach (var domainEventType in domainEventTypes)
        {
            var eventSubscriberInterfaceType = typeof(IEventSubscriber<>).MakeGenericType(domainEventType);
            var eventSubscriberClassTypeList = typeFinder.FindClassesOfType(eventSubscriberInterfaceType).ToList();
            _eventHandlerMappingCache.TryAdd(domainEventType.FullName.ToLower(), value: new EventHandlerMappingItem(domainEventType, eventSubscriberClassTypeList));
        }
    }

    /// <summary>
    /// GetEventType
    /// </summary>
    /// <param name="commandTypeKey"></param>
    /// <returns></returns>
    public Type GetEventType(string commandTypeKey)
    {
        return _eventHandlerMappingCache.ContainsKey(commandTypeKey) ? _eventHandlerMappingCache[commandTypeKey].EventType : null;
    }

    /// <summary>
    /// GetEventSubscriberTypeList
    /// </summary>
    /// <param name="commandTypeKey"></param>
    /// <returns></returns>
    public List<Type> GetEventSubscriberTypeList(string commandTypeKey)
    {
        return _eventHandlerMappingCache.ContainsKey(commandTypeKey) ? _eventHandlerMappingCache[commandTypeKey].EventSubscribedTypeList : null;
    }

    /// <summary>
    /// EventHandlerMappingItem
    /// </summary>
    class EventHandlerMappingItem
    {
        public Type EventType { get; }
        public List<Type> EventSubscribedTypeList { get; }

        public EventHandlerMappingItem(Type domainEventType, List<Type> commandProcessorType)
        {
            EventType = domainEventType;
            EventSubscribedTypeList = commandProcessorType;
        }
    }
}

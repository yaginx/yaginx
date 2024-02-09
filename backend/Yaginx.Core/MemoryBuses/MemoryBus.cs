using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yaginx.MemoryBuses.Events;

namespace Yaginx.MemoryBuses;

public class MemoryBus : IMemoryBus
{
    public async Task SendAsync(Event @event, Dictionary<string, string> optionalHeaders = null, bool isLongTimeMessage = false, bool isShareWorkContext = true)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        @event.TimeStamp = DateTime.Now;
        @event.TaskId = Guid.NewGuid().ToString();
        var mainKey = isLongTimeMessage ? "app.longtimeevent" : "app.event";
        var routingKey = $"{mainKey}.{@event.GetType().FullName.ToLower()}";
        var serializedMessageBody = JsonConvert.SerializeObject(@event);

        if (AgileLabContexts.Context.CurrentWorkContext == null)
        {
            isShareWorkContext = false;// 如果当前上下文不存在WorkContext,强制转为异步处理
        }

        //fire event
        if (isShareWorkContext)
        {
            var _eventConsumer = AgileLabContexts.Context.CurrentWorkContext.Resolve<IEventConsumer>();
            await _eventConsumer.ConsumerEvent(routingKey, serializedMessageBody);
        }
        else
        {
            _ = Task.Factory.StartNew(async () =>
            {
                using var scope = AgileLabContexts.Context.CreateScopeWithWorkContextForNewTask();
                var localEventConsumer = scope.WorkContext.Resolve<IEventConsumer>();
                var loggerFactory = scope.WorkContext.Resolve<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger($"EventProcessor(EventType->{@event.GetType().FullName}):");
                try
                {
                    await localEventConsumer.ConsumerEvent(routingKey, serializedMessageBody);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Async EventProcessor处理异常");
                }
            }, isLongTimeMessage ? TaskCreationOptions.LongRunning : TaskCreationOptions.None);
        }
    }

    public void Send(Event @event, Dictionary<string, string> optionalHeaders, bool isLongTimeMessage, bool isShareWorkContext = true)
    {
        try
        {
            SendAsync(@event, optionalHeaders, isLongTimeMessage, isShareWorkContext).Wait();
        }
        catch (AggregateException exception)
        {
            if (exception.InnerExceptions?.Count == 1)
                throw exception.InnerExceptions.First();
            else
                throw;
        }
    }
}

using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using Yaginx.MemoryBuses.Events;

namespace Yaginx.MemoryBuses;

public class MemoryEventConsumer : IEventConsumer
{
    private const string EVENT_KEY_PREFIX = "app.event.";
    private readonly IEventHandlerMappingCacheManager _eventHandlerMappingManager;
    private readonly ILogger<MemoryEventConsumer> _logger;
    private readonly IWorkContextCore _workContext;

    public MemoryEventConsumer(IEventHandlerMappingCacheManager eventHandlerMappingManager,
        ILogger<MemoryEventConsumer> logger, IWorkContextCore workContext)
    {
        _eventHandlerMappingManager = eventHandlerMappingManager ?? throw new ArgumentNullException(nameof(eventHandlerMappingManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _workContext = workContext;
    }

    public async Task ConsumerEvent(string routingKey, string bodyMessage)
    {
        var eventTypeKey = routingKey.Substring(EVENT_KEY_PREFIX.Length);
        if (string.IsNullOrWhiteSpace(eventTypeKey))
        {
            throw new Exception($"无法根据RoutingKey识别当前消息, RoutingKey:[{routingKey}]");
        }

        var eventType = _eventHandlerMappingManager.GetEventType(eventTypeKey);
        if (eventType == null)
        {
            throw new Exception($"未找到EventType:[{eventTypeKey}]");
        }

        if (!(JsonConvert.DeserializeObject(bodyMessage, eventType) is Event eventMessage))
        {
            throw new Exception($"EventMessage实体反序列化失败, CommandType:[{eventType.FullName}]");
        }

        _logger.LogInformation($"开始处理事件: {eventType.FullName} #[{eventMessage.EventId}]");
        var messageHandleResult = new MessageHandlerResult()
        {
            MessageId = eventMessage.EventId,
            RoutingKey = routingKey,
            MessageType = (int)MessageTypeEnum.Event,
            MessageBody = bodyMessage,
            HandlerNode = Environment.MachineName,
            Logged = DateTime.Now,
            HandleResult = (int)HandleResultEnum.Success
        };

        var subscriberTypeList = _eventHandlerMappingManager.GetEventSubscriberTypeList(eventTypeKey);
        if (subscriberTypeList.Any())
        {
            List<Task> eventTasks = new List<Task>();
            List<string> errMessage = new List<string>();
            List<Exception> consumerExceptionList = new List<Exception>(subscriberTypeList.Count);
            //触发各个订阅，事件的订阅端多线程执行，不区分执行顺序
            foreach (var eventSubscriberType in subscriberTypeList)
            {
                //using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
                //var _workContext = scope.WorkContext;
                try
                {
                    //针对不同的EventHandler使用不同的serviceProvider, 避免各个EventHandler中的实例冲突, 保证执行的实例安全
                    var eventSubscriber = _workContext.Resolve(eventSubscriberType);
                    var processMethod = eventSubscriberType.GetMethod("Handle", new[] { eventType });

                    var task = (Task)processMethod.Invoke(eventSubscriber, new object[] { eventMessage });
                    task.Wait(TimeSpan.FromMinutes(5));//单个event handler最多允许执行5分钟
                }
                catch (AggregateException exception)
                {
                    if (exception.InnerExceptions.Any())
                    {
                        consumerExceptionList.AddRange(exception.InnerExceptions);
                    }
                    else if (exception.InnerException != null)
                    {
                        consumerExceptionList.Add(exception);
                    }
                    else
                    {
                        consumerExceptionList.Add(exception);
                    }
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        consumerExceptionList.Add(exception.InnerException);
                    }
                    else
                    {
                        consumerExceptionList.Add(exception);
                    }
                }
                catch (Exception exception)
                {
                    consumerExceptionList.Add(exception);
                    var errMsgItem = $"Event:{eventType.Name}#{(eventMessage?.EventId.ToString()) ?? "NOEVENTID"}的订阅方{eventSubscriberType.Name}处理中出现异常, 其他订阅者继续执行, {exception.FullMessage()}";
                    errMessage.Add(errMsgItem);
                    _logger.LogError(exception, errMsgItem);
                }
            }
            if (errMessage.Any())
            {
                messageHandleResult.HandleResult = (int)HandleResultEnum.PartialSuccess;
                messageHandleResult.ResultMessage = JsonConvert.SerializeObject(errMessage);
            }

            if (consumerExceptionList.Any())
            {
                if (consumerExceptionList.Count == 1)
                    throw consumerExceptionList.First();
                else
                    throw new AggregateException(consumerExceptionList);
            }
        }
        if (messageHandleResult.HandleResult != (int)HandleResultEnum.Success)
        {
            //using (var serviceProviderScope = _serviceProvider.CreateScope())
            //{
            //    var scopedServiceProvider = serviceProviderScope.ServiceProvider;
            //    var _messageHandlerResultRepository = scopedServiceProvider.GetRequiredService<IAppNoSqlBaseRepository<MessageHandlerResult>>();
            //    await _messageHandlerResultRepository.AddAsync(messageHandleResult);
            //}
        }
        _logger.LogInformation($"结束处理事件: {eventType.FullName} #[{eventMessage.EventId}]");
        await Task.CompletedTask;
    }
}

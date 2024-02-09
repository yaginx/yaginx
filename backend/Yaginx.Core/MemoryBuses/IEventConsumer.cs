namespace Yaginx.MemoryBuses;

public interface IEventConsumer
{
    Task ConsumerEvent(string routingKey, string bodyMessage);
}
using Yaginx.MemoryBuses.Messages;

namespace Yaginx.MemoryBuses.Events;

// Summary:
//     Provides the base implementation for Memento's domain events
public abstract class Event : Message, IEvent
{
    //
    // Summary:
    //     Creates a DomainEvent instance
    public Event()
    {
        EventId = Guid.NewGuid();
    }

    public Guid EventId { get; set; }

    //
    // Summary:
    //     Get or set the time at which the event occurred
    public DateTime TimeStamp { get; set; }
    //
    // Summary:
    //     Gets or sets the TimelineId
    public Guid? TimelineId { get; set; }
}

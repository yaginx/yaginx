using System.Runtime.Serialization;
using Yaginx.MemoryBuses.Events;

namespace Yaginx.Infrastructure;

public class MonitorMessage : Event
{
    [DataMember(Name = "ts")]
    public long ts { get; set; }
    [DataMember(Name = "data")]
    public List<MonitorRawInfo> data { get; set; }
}
using AgileLabs.MemoryBuses.Events;
using System.Runtime.Serialization;

namespace Yaginx.Infrastructure;

public class MonitorMessage : Event
{
    [DataMember(Name = "ts")]
    public long ts { get; set; }
    [DataMember(Name = "data")]
    public List<MonitorRawInfo> data { get; set; }
}
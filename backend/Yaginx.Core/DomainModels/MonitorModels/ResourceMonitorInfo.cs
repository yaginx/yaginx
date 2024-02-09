namespace Yaginx.DomainModels.MonitorModels;

public class ResourceMonitorInfo
{
    public string ResourceUuid { get; set; }
    public string SessionKey { get; set; }
    public DateTime Timestamp { get; set; }
    public List<MonitorInfo> Data { get; set; }
}

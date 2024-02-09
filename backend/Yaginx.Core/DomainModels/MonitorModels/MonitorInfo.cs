namespace Yaginx.DomainModels.MonitorModels;

public class MonitorInfo
{

    public UserAgentInfo Ua { get; set; }

    public DeviceInfo Device { get; set; }

    public OsInfo Os { get; set; }

    public string Host { get; set; }
    public string Path { get; set; }
    public string QueryString { get; set; }
    public string Referer { get; set; }

    public string UserAgent { get; set; }

    public string Ip { get; set; }

    public string Lang { get; set; }

    public int StatusCode { get; set; }

    public long Duration { get; set; }
}

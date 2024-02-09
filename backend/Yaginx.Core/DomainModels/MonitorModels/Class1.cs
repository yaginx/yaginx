using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaginx.Infrastructure;

namespace Yaginx.DomainModels.MonitorModels;

public interface IMonitorInfoRepository
{
    Task AddAsync(ResourceMonitorInfo monitorInfoEntity);
}
public class ResourceMonitorProfile : Profile
{
    public ResourceMonitorProfile()
    {
        CreateMap<MonitorMessage, ResourceMonitorInfo>();
        CreateMap<MonitorRawInfo, MonitorInfo>();
    }
}
public class ResourceMonitorInfo
{
    public string ResourceUuid { get; set; }
    public string SessionKey { get; set; }
    public DateTime Timestamp { get; set; }
    public List<MonitorInfo> Data { get; set; }
}
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
public class UserAgentInfo
{
    public string Family { get; set; }
    public string Major { get; set; }
}

public class DeviceInfo
{
    //
    // 摘要:
    //     Returns true if the device is likely to be a spider or a bot device
    public bool IsSpider { get; set; }
    //
    // 摘要:
    //     The brand of the device
    public string Brand { get; set; }
    //
    // 摘要:
    //     The family of the device, if available
    public string Family { get; set; }
    //
    // 摘要:
    //     The model of the device, if available
    public string Model { get; set; }
}

public class OsInfo
{
    //
    // 摘要:
    //     The familiy of the OS
    public string Family { get; set; }
    //
    // 摘要:
    //     The major version of the OS, if available
    public string Major { get; set; }
}

using AutoMapper;
using Yaginx.Infrastructure;

namespace Yaginx.DomainModels.MonitorModels;

public class ResourceMonitorProfile : Profile
{
    public ResourceMonitorProfile()
    {
        CreateMap<MonitorMessage, ResourceMonitorInfo>();
        CreateMap<MonitorRawInfo, MonitorInfo>();
    }
}

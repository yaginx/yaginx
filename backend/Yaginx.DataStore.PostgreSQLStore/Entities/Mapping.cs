using Yaginx.DomainModels;
using AutoMapper;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Entities
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<WebsiteDomainModel, WebsiteEntity>().ReverseMap();
            CreateMap<User, AccountEntity>().ReverseMap();
            CreateMap<WebDomain, WebDomainEntity>().ReverseMap();
            CreateMap<HostTraffic, HostTrafficEntity>().ReverseMap();

            CreateMap<ResourceMonitorInfo, ResourceMonitorInfoEntity>().ReverseMap();
            CreateMap<ResourceReportModel, ResourceReportEntity>().ReverseMap();
        }
    }
}

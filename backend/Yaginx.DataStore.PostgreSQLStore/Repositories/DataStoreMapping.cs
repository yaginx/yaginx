using AutoMapper;
using Yaginx.DataStore.PostgreSQLStore.Entities;
using Yaginx.DomainModels.MonitorModels;

namespace Yaginx.DataStore.PostgreSQLStore.Repositories
{
    public class DataStoreMapping : Profile
    {
        public DataStoreMapping()
        {
            CreateMap<ResourceMonitorInfo, ResourceMonitorInfoEntity>()
                .ForMember(d => d.ResourceUuid, mo => mo.MapFrom(s => "yaginx"));

            //CreateMap<MonitorInfo, MonitorInfoEntity>();

            CreateMap<ResourceReportEntity, ResourceReportModel>();
        }
    }
}

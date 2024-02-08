using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.Models.TrafficModels
{
    public class TrafficMapping : Profile
    {
        public TrafficMapping()
        {
            CreateMap<HostTraffic, HostTrafficListItem>();
        }
    }
}

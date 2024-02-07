using AgileLabs;
using AutoMapper;
using Docker.DotNet.Models;

namespace Yaginx.Models.DockerModels
{
    public class ContainerListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public DateTime Created { get; set; }
        public string Ports { get; set; }
        public string Network { get; set; }
        public string Mounts { get; set; }
    }

    public class ContainerMapping : Profile
    {
        public ContainerMapping()
        {
            CreateMap<ContainerListResponse, ContainerListItem>()
                .ForMember(d => d.Name, mo => mo.MapFrom(s => s.Names.FirstOrDefault()))
                .ForMember(d => d.Ports, mo => mo.MapFrom(s => s.Ports.Select(x => $"{x.PublicPort}:{x.PrivatePort}").JoinStrings(",")))
                .ForMember(d => d.Mounts, mo => mo.MapFrom(s => s.Mounts.Select(x => $"{x.Source}:{x.Destination}({x.RW})").JoinStrings(",")))
                .ForMember(d => d.Network, mo => mo.MapFrom(s => s.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress ?? string.Empty));
        }
    }
}

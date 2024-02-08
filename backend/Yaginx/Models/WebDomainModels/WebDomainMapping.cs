using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.Models.WebDomainModels
{
    public class WebDomainMapping : Profile
    {
        public WebDomainMapping()
        {
            CreateMap<WebDomain, WebDomainListItem>()
                .ForMember(d => d.IsHaveCert, mo => mo.MapFrom<DomainHasCertResolver, string>(s => s.Name));
        }
    }
}

using AutoMapper;
using Yaginx.DomainModels;
using Yaginx.Models.WebDomainModels;

namespace Yaginx.Models.WebsiteModels
{
    public class WebsiteMapping : Profile
    {
        public WebsiteMapping()
        {
            CreateMap<WebsiteUpsertRequest, Website>();
            CreateMap<Website, WebsiteListItem>()
                .ForMember(d => d.IsHaveSslCert, mo => mo.MapFrom<DomainHasCertResolver, string>(s => s.DefaultHost));
        }
    }
}

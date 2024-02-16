using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.Models.WebsiteModels
{
    public class WebsiteMapping : Profile
    {
        public WebsiteMapping()
        {
            CreateMap<WebsiteUpsertRequest, Website>();
            CreateMap<Website, WebsiteListItem>();
        }
    }
}

using AutoMapper;
using Yaginx.DomainModels;

namespace Yaginx.Models
{
    public class WebsiteMapping : Profile
    {
        public WebsiteMapping()
        {
            CreateMap<WebsiteUpsertRequest, Website>();
        }
    }
}

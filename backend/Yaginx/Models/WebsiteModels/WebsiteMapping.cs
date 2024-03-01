using AutoMapper;
using Yaginx.DomainModels;
using Yaginx.Models.WebDomainModels;

namespace Yaginx.Models.WebsiteModels
{
    public class WebsiteMapping : Profile
    {
        public WebsiteMapping()
        {
            CreateMap<WebsiteUpsertRequest, Website>()
                .ForMember(d => d.Specifications, mo => mo.MapFrom(s => new WebsiteSpecifications
                {
                    DefaultHost = s.DefaultHost,
                    DefaultDestination = s.DefaultDestination,
                    DefaultDestinationHost = s.DefaultDestinationHost,
                    IsAllowUnsafeSslCertificate = s.IsAllowUnsafeSslCertificate,
                    IsAutoRedirectHttp2Https = s.IsAutoRedirectHttp2Https,
                    IsWithOriginalHostHeader = s.IsWithOriginalHostHeader,
                    WebProxy = s.WebProxy
                }));

            CreateMap<Website, WebsiteUpsertRequest>()
                .ForMember(d => d.DefaultHost, mo => mo.MapFrom(s => s.Specifications.DefaultHost))
                .ForMember(d => d.DefaultDestination, mo => mo.MapFrom(s => s.Specifications.DefaultDestination))
                .ForMember(d => d.DefaultDestinationHost, mo => mo.MapFrom(s => s.Specifications.DefaultDestinationHost))
                .ForMember(d => d.IsAllowUnsafeSslCertificate, mo => mo.MapFrom(s => s.Specifications.IsAllowUnsafeSslCertificate))
                .ForMember(d => d.IsAutoRedirectHttp2Https, mo => mo.MapFrom(s => s.Specifications.IsAutoRedirectHttp2Https))
                .ForMember(d => d.IsWithOriginalHostHeader, mo => mo.MapFrom(s => s.Specifications.IsWithOriginalHostHeader))
                .ForMember(d => d.WebProxy, mo => mo.MapFrom(s => s.Specifications.WebProxy));

            CreateMap<Website, WebsiteListItem>()
                .ForMember(d => d.DefaultHost, mo => mo.MapFrom(s => s.Specifications.DefaultHost))
                .ForMember(d => d.DefaultDestination, mo => mo.MapFrom(s => s.Specifications.DefaultDestination))
                .ForMember(d => d.DefaultDestinationHost, mo => mo.MapFrom(s => s.Specifications.DefaultDestinationHost))
                .ForMember(d => d.IsAllowUnsafeSslCertificate, mo => mo.MapFrom(s => s.Specifications.IsAllowUnsafeSslCertificate))
                .ForMember(d => d.IsAutoRedirectHttp2Https, mo => mo.MapFrom(s => s.Specifications.IsAutoRedirectHttp2Https))
                .ForMember(d => d.IsWithOriginalHostHeader, mo => mo.MapFrom(s => s.Specifications.IsWithOriginalHostHeader))
                .ForMember(d => d.WebProxy, mo => mo.MapFrom(s => s.Specifications.WebProxy))
                .ForMember(d => d.IsHaveSslCert, mo => mo.MapFrom<DomainHasCertResolver, string>(s => s.Specifications.DefaultHost));
        }
    }
}

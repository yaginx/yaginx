using AutoMapper;
using LettuceEncrypt.Internal;
using Yaginx.DomainModels;

namespace Yaginx.Models.WebDomainModels
{
    public class DomainHasCertResolver : IMemberValueResolver<WebDomain, WebDomainListItem, string, bool>
    {
        private readonly CertificateSelector _certificateSelector;

        public DomainHasCertResolver(CertificateSelector certificateSelector)
        {
            _certificateSelector = certificateSelector;
        }

        public bool Resolve(WebDomain source, WebDomainListItem destination, string sourceMember, bool destMember, ResolutionContext context)
        {
            return _certificateSelector.HasCertForDomain(source.Name);
        }
    }
}

using AutoMapper;
using LettuceEncrypt.Internal;

namespace Yaginx.Models.WebDomainModels
{
    public class DomainHasCertResolver : IMemberValueResolver<object, object, string, bool>
    {
        private readonly CertificateSelector _certificateSelector;

        public DomainHasCertResolver(CertificateSelector certificateSelector)
        {
            _certificateSelector = certificateSelector;
        }

        public bool Resolve(object source, object destination, string sourceMember, bool destMember, ResolutionContext context)
        {
            return _certificateSelector.HasCertForDomain(sourceMember);
        }
    }
}

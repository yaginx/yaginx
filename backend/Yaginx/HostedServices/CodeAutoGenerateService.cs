using AgileLabs;
using Yaginx.DomainModels;

namespace Yaginx.HostedServices
{
    public class CodeAutoGenerateService : IScoped
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebDomainRepository _webDomainRepository;

        public CodeAutoGenerateService(IServiceProvider serviceProvider, IWebDomainRepository webDomainRepository)
        {
            _serviceProvider = serviceProvider;
            _webDomainRepository = webDomainRepository;
        }

        public async Task UserCodeGenerateAsync()
        {

        }
    }
}
using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Yaginx.DomainModels;
using Yaginx.YaginxAcmeLoaders;

namespace Yaginx;

public class CertificateDomainSingletonService : ICertificateDomainRepsitory
{
    public async Task<IEnumerable<string>> GetFreeCertDomainAsync()
    {
        using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
        var _webDomainRepository = scope.Resolve<IWebDomainRepository>();
        var result = await _webDomainRepository.SearchAsync(true);
        return result.Select(x => x.Name);
    }

    public async Task UnFreeDomainAsync(string domain, string message)
    {
        using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
        var _webDomainRepository = scope.Resolve<IWebDomainRepository>();
        var dbCommiter = scope.Resolve<IDbContextCommiter>();

        var webDomain = await _webDomainRepository.GetByNameAsync(domain);
        webDomain.IsUseFreeCert = false;
        webDomain.FreeCertMessage = message;
        await _webDomainRepository.UpdateAsync(webDomain);
        await dbCommiter.CommitAsync();
    }
}

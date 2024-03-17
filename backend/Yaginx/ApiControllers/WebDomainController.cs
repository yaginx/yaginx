using AgileLabs.EfCore.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Models.WebDomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("yaginx/api/web_domain")]
public class WebDomainController : YaginxControllerBase
{
    private readonly IWebDomainRepository _webDomainRepository;

    public WebDomainController(IWebDomainRepository webDomainRepository)
    {
        _webDomainRepository = webDomainRepository;
    }

    [HttpPost, Route("upsert")]
    public async Task<WebDomain> Upsert([FromBody] WebDomain webDomain)
    {
        webDomain.Name = webDomain.Name.ToLower();
        var oldDomain = await _webDomainRepository.GetByNameAsync(webDomain.Name);
        if (oldDomain == null)
        {
            webDomain.Id = IdGenerator.NextId();
            await _webDomainRepository.AddAsync(webDomain);
            return webDomain;
        }
        else
        {
            oldDomain.IsUseFreeCert = webDomain.IsUseFreeCert;
            oldDomain.IsVerified = webDomain.IsVerified;
            await _webDomainRepository.UpdateAsync(oldDomain);
            return oldDomain;
        }
    }

    [HttpDelete, Route("delete")]
    public async Task Delete(long id)
    {
        await _webDomainRepository.DeleteAsync(id);
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebDomainListItem>> Search()
    {
        var source = await _webDomainRepository.SearchAsync();
        return _mapper.Map<List<WebDomainListItem>>(source.ToList());
    }

    [HttpGet, Route("get")]
    public async Task<WebDomainListItem> Get(long id)
    {
        var result = await _webDomainRepository.GetAsync(id);
        return _mapper.Map<WebDomainListItem>(result);
    }

    [HttpGet, Route("get_by_name")]
    public async Task<WebDomainListItem> Get(string name)
    {
        var result = await _webDomainRepository.GetByNameAsync(name);
        return _mapper.Map<WebDomainListItem>(result);
    }
}

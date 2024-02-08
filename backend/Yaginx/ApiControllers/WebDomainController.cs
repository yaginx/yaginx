using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/web_domain")]
public class WebDomainController : YaginxControllerBase
{
    private readonly IWebDomainRepository _webDomainRepository;

    public WebDomainController(IWebDomainRepository webDomainRepository)
    {
        _webDomainRepository = webDomainRepository;
    }

    [HttpPost, Route("upsert")]
    public async Task Upsert([FromBody] WebDomain webDomain)
    {
        webDomain.Name = webDomain.Name.ToLower();
        var oldDomain = _webDomainRepository.GetByName(webDomain.Name);
        if (oldDomain == null)
        {
            webDomain.Id = IdGenerator.NextId();
            _webDomainRepository.Add(webDomain);
        }
        else
        {
            _webDomainRepository.Update(webDomain);
        }
        await Task.CompletedTask;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebDomain>> Search()
    {
        await Task.CompletedTask;
        return _webDomainRepository.Search().ToList();
    }

    [HttpGet, Route("get")]
    public async Task<WebDomain> Get(string name)
    {
        await Task.CompletedTask;
        return _webDomainRepository.GetByName(name);
    }
}

using LettuceEncrypt.Internal;
using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Models.WebDomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/web_domain")]
public class WebDomainController : YaginxControllerBase
{
    private readonly IWebDomainRepository _webDomainRepository;
    private readonly CertificateSelector _certificateSelector;

    public WebDomainController(IWebDomainRepository webDomainRepository, CertificateSelector certificateSelector)
    {
        _webDomainRepository = webDomainRepository;
        _certificateSelector = certificateSelector;
    }

    [HttpPost, Route("upsert")]
    public async Task<WebDomain> Upsert([FromBody] WebDomain webDomain)
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
        return webDomain;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebDomainListItem>> Search()
    {
        await Task.CompletedTask;
        return _mapper.Map<List<WebDomainListItem>>(_webDomainRepository.Search().ToList());
    }

    [HttpGet, Route("get")]
    public async Task<WebDomainListItem> Get(long id)
    {
        await Task.CompletedTask;
        return _mapper.Map<WebDomainListItem>(_webDomainRepository.Get(id));
    }

    [HttpGet, Route("get_by_name")]
    public async Task<WebDomainListItem> Get(string name)
    {
        await Task.CompletedTask;
        return _mapper.Map<WebDomainListItem>(_webDomainRepository.GetByName(name));
    }
}

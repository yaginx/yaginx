using LettuceEncrypt.Internal;
using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Models.WebDomainModels;

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
    public async Task<WebDomain> Upsert([FromBody] WebDomain webDomain)
    {
        webDomain.Name = webDomain.Name.ToLower();
        var oldDomain = _webDomainRepository.GetByNameAsync(webDomain.Name);
        if (oldDomain == null)
        {
            webDomain.Id = IdGenerator.NextId();
            await _webDomainRepository.AddAsync(webDomain);
        }
        else
        {
            await _webDomainRepository.UpdateAsync(webDomain);
        }
        await Task.CompletedTask;
        return webDomain;
    }

    [HttpDelete, Route("delete")]
    public async Task Delete(long id)
    {
        await _webDomainRepository.DeleteAsync(id);
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebDomainListItem>> Search()
    {
        await Task.CompletedTask;
        IEnumerable<WebDomain> source = await _webDomainRepository.SearchAsync();
        return _mapper.Map<List<WebDomainListItem>>(source.ToList());
    }

    [HttpGet, Route("get")]
    public async Task<WebDomainListItem> Get(long id)
    {
        await Task.CompletedTask;
        return _mapper.Map<WebDomainListItem>(_webDomainRepository.GetAsync(id));
    }

    [HttpGet, Route("get_by_name")]
    public async Task<WebDomainListItem> Get(string name)
    {
        await Task.CompletedTask;
        return _mapper.Map<WebDomainListItem>(_webDomainRepository.GetByNameAsync(name));
    }
}

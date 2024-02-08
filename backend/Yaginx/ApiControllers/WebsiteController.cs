using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.Models;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/website")]
public class WebsiteController : YaginxControllerBase
{
    private readonly IWebsiteRepository _websiteRepository;
    private readonly ProxyRuleChangeNotifyService _proxyRuleChangeNotifyService;

    public WebsiteController(IWebsiteRepository websiteRepository, ProxyRuleChangeNotifyService proxyRuleChangeNotifyService)
    {
        _websiteRepository = websiteRepository;
        _proxyRuleChangeNotifyService = proxyRuleChangeNotifyService;
    }

    [HttpPost, Route("upsert")]
    public async Task<Website> Add([FromBody] WebsiteUpsertRequest request)
    {
        Website returnValue = null;
        if (!request.Id.HasValue)
        {
            var site = _mapper.Map<Website>(request);
            site.Id = IdGenerator.NextId();
            site.CreateTime = DateTime.Now;
            site.UpdateTime = DateTime.Now;
            _websiteRepository.Add(site);
            returnValue = site;
        }
        else
        {
            var oldSite = _websiteRepository.Get(request.Id.Value);
            if (oldSite == null)
            {
                throw new Exception($"Site #{request.Id} not exist");
            }

            _mapper.Map(request, oldSite);

            oldSite.UpdateTime = DateTime.Now;
            _websiteRepository.Update(oldSite);
            returnValue = oldSite;
        }
        await Task.CompletedTask;
        _proxyRuleChangeNotifyService.ProxyRuleConfigChanged();
        return returnValue;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<Website>> Search()
    {
        await Task.CompletedTask;
        return _websiteRepository.Search();
    }

    [HttpGet, Route("get")]
    public async Task<Website> Get(long id)
    {
        await Task.CompletedTask;
        return _websiteRepository.Get(id);
    }

    [HttpGet, Route("get_by_name")]
    public async Task<Website> Get(string name)
    {
        await Task.CompletedTask;
        return _websiteRepository.GetByName(name);
    }
}

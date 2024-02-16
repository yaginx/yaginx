using Microsoft.AspNetCore.Mvc;
using Yaginx.DataStore.LiteDBStore.Repositories;
using Yaginx.DomainModels;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.Models.WebsiteModels;

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
        Website returnValue;
        if (!request.Id.HasValue)
        {
            var site = _mapper.Map<Website>(request);
            site.Id = IdGenerator.NextId();
            site.CreateTime = DateTime.Now;
            site.UpdateTime = DateTime.Now;
            await _websiteRepository.AddAsync(site);
            returnValue = site;
        }
        else
        {
            var oldSite = await _websiteRepository.GetAsync(request.Id.Value);
            if (oldSite == null)
            {
                throw new Exception($"Site #{request.Id} not exist");
            }

            _mapper.Map(request, oldSite);

            oldSite.UpdateTime = DateTime.Now;
            await _websiteRepository.UpdateAsync(oldSite);
            returnValue = oldSite;
        }
        _proxyRuleChangeNotifyService.ProxyRuleConfigChanged();
        return returnValue;
    }

    [HttpDelete, Route("delete")]
    public async Task Delete(long id)
    {
        await _websiteRepository.DeleteAsync(id);
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebsiteListItem>> Search()
    {
        var result = await _websiteRepository.SearchAsync();
        return _mapper.Map<List<WebsiteListItem>>(result);
    }

    [HttpGet, Route("get")]
    public async Task<WebsiteListItem> Get(long id)
    {
        var entity = await _websiteRepository.GetAsync(id);
        return _mapper.Map<WebsiteListItem>(entity);
    }

    [HttpGet, Route("get_by_name")]
    public async Task<WebsiteListItem> Get(string name)
    {
        var entity = await _websiteRepository.GetByNameAsync(name);
        return _mapper.Map<WebsiteListItem>(entity);
    }
}

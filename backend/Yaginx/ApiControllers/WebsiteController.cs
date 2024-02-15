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
    public async Task<List<Website>> Search()
    {
        var result = await _websiteRepository.SearchAsync();
        return result.ToList();
    }

    [HttpGet, Route("get")]
    public async Task<Website> Get(long id)
    {
        return await _websiteRepository.GetAsync(id);
    }

    [HttpGet, Route("get_by_name")]
    public async Task<Website> Get(string name)
    {
        return await _websiteRepository.GetByNameAsync(name);
    }
}

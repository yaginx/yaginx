using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/website")]
public class WebsiteController : YaginxControllerBase
{
    private readonly IWebsiteRepository _websiteRepository;

    public WebsiteController(IWebsiteRepository websiteRepository)
    {
        _websiteRepository = websiteRepository;
    }

    [HttpPost, Route("upsert")]
    public async Task Add([FromBody] Website site)
    {
        if (!site.Id.HasValue)
        {
            site.Id = IdGenerator.NextId();
            _websiteRepository.Add(site);
        }
        else
        {
            var oldSite = _websiteRepository.Get(site.Id.Value);
            if (oldSite == null)
            {
                throw new Exception($"Site #{site.Id} not exist");
            }
            _websiteRepository.Update(site);
        }
        await Task.CompletedTask;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<Website>> Search()
    {
        await Task.CompletedTask;
        return _websiteRepository.Search();
    }

    [HttpGet, Route("get")]
    public async Task<Website> Get(string name)
    {
        await Task.CompletedTask;
        return _websiteRepository.GetByName(name);
    }
}

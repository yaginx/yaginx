using Microsoft.AspNetCore.Mvc;
using Yaginx.DataStores;
using Yaginx.DomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/website")]
public class WebsiteController : YaginxControllerBase
{
    [HttpPost, Route("upsert")]
    public async Task Add([FromBody] Website site)
    {
        if (!site.Id.HasValue)
        {
            site.Id = IdGenerator.NextId();
            _databaseRepository.Insert(site);
        }
        else
        {
            var oldSite = _databaseRepository.Get<Website>(x => x.Id == site.Id);
            if (oldSite == null)
            {
                throw new Exception($"Site #{site.Id} not exist");
            }
            _databaseRepository.Update(site);
        }
        await Task.CompletedTask;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<Website>> Search()
    {
        await Task.CompletedTask;
        return _databaseRepository.Search<Website>().ToList();
    }

    [HttpGet, Route("get")]
    public async Task<Website> Get(string name)
    {
        await Task.CompletedTask;
        return _databaseRepository.Get<Website>(x => x.Name == name);
    }
}

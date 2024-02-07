using Microsoft.AspNetCore.Mvc;
using Yaginx.DataStores;
using Yaginx.DomainModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/web_domain")]
public class WebDomainController : YaginxControllerBase
{
    [HttpPost, Route("upsert")]
    public async Task Upsert([FromBody] WebDomain webDomain)
    {
        if (!webDomain.Id.HasValue)
        {
            webDomain.Id = IdGenerator.NextId();
            _databaseRepository.Insert(webDomain);
        }
        else
        {
            var oldDomain = _databaseRepository.Get<WebDomain>(x => x.Id == webDomain.Id);
            if (oldDomain == null)
            {
                throw new Exception($"Site #{webDomain.Id} not exist");
            }
            _databaseRepository.Update(webDomain);
        }
        await Task.CompletedTask;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<WebDomain>> Search()
    {
        await Task.CompletedTask;
        return _databaseRepository.Search<WebDomain>().ToList();
    }

    [HttpGet, Route("get")]
    public async Task<WebDomain> Get(string name)
    {
        await Task.CompletedTask;
        return _databaseRepository.Get<WebDomain>(x => x.Name == name);
    }
}

using AgileLabs.ComponentModels;
using AgileLabs.DynamicSearch;
using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Infrastructure;
using Yaginx.Models.TrafficModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("yaginx/api/host_traffic")]
public class HostTrafficController : YaginxControllerBase
{
    private readonly IHostTrafficRepository _hostTrafficRepository;

    public HostTrafficController(IHostTrafficRepository hostTrafficRepository)
    {
        _hostTrafficRepository = hostTrafficRepository;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<Page<HostTrafficListItem>> Search([FromBody, ModelBinder<AntdTableSearchParametersBinder>] SearchParameters searchParameters)
    {
        var result = await _hostTrafficRepository.SearchAsync(searchParameters);
        return _mapper.Map<Page<HostTrafficListItem>>(result);
    }
}

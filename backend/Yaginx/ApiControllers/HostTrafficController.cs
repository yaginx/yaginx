using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;
using Yaginx.Models.TrafficModels;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/host_traffic")]
public class HostTrafficController : YaginxControllerBase
{
    private readonly IHostTrafficRepository _hostTrafficRepository;

    public HostTrafficController(IHostTrafficRepository hostTrafficRepository)
    {
        _hostTrafficRepository = hostTrafficRepository;
    }

    [HttpGet, HttpPost, Route("search")]
    public async Task<List<HostTrafficListItem>> Search()
    {
        var result = await _hostTrafficRepository.SearchAsync();
        return _mapper.Map<List<HostTrafficListItem>>(result);
    }
}

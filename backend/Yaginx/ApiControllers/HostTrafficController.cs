using Microsoft.AspNetCore.Mvc;
using Yaginx.DomainModels;

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
    public async Task<List<HostTraffic>> Search()
    {
        await Task.CompletedTask;
        return _hostTrafficRepository.Search();
    }
}

using AgileLabs.MemoryBuses;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Yaginx.Models;
using Yaginx.Models.DockerModels;
using Yaginx.Services.DockerServices;

namespace Yaginx.ApiControllers.DockerControllers;
[ApiController, Route("yaginx/api/docker/container")]
public class ContainerController : YaginxControllerBase
{
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<ContainerController> _logger;

    public ContainerController(IDockerClient dockerClient, ILogger<ContainerController> logger)
    {
        _dockerClient = dockerClient;
        _logger = logger;
    }

    [HttpGet, Route("search")]
    public async Task<List<ContainerListItem>> Search()
    {
        IList<ContainerListResponse> containers = await _dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = int.MaxValue,
                //Filters = "id=test&test=sdf"
            });

        return _mapper.Map<List<ContainerListItem>>(containers);
    }

    [HttpGet, Route("get")]
    public async Task<ContainerListItem> Detail(string id)
    {
        ContainerListResponse? image = await GetContainerByContainerId(HttpUtility.UrlDecode(id));
        if (image == null)
            throw new Exception("Not Found");

        return _mapper.Map<ContainerListItem>(image);
    }

    [AllowAnonymous]
    [HttpGet, Route("sample")]
    public async Task<ReplaceNewImageRequest> GetSample()
    {
        var newContainerJson = new ReplaceNewImageRequest();

        newContainerJson.Name = "redis";
        newContainerJson.Image = "redis:latest";
        newContainerJson.Envs = new List<string>() { "Environment=Production" };
        newContainerJson.Volumns = new List<string>() { "/data:/data" };
        newContainerJson.Ports = new List<KeyValuePair<string, List<string>>>();
        newContainerJson.Ports.Add(new KeyValuePair<string, List<string>>("6379", new List<string> { "6379" }));
        await Task.CompletedTask;
        return newContainerJson;
    }

    [HttpPost, Route("replace")]
    public async Task ReplaceImage([FromBody] ReplaceNewImageRequest request, [FromServices] ContainerServcie containerServcie, [FromServices] IMemoryBus memoryBus)
    {
        await containerServcie.ReplaceImage(request);
    }

    private async Task<ContainerListResponse?> GetContainerByContainerId(string containerId)
    {
        IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>();
        filters.Add("id", new Dictionary<string, bool> { { $"{containerId}", true } });

        var images = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
        {
            Filters = filters,
        }, HttpContext.RequestAborted);

        return images.FirstOrDefault();
    }
}

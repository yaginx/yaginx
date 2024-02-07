using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;

namespace Yaginx.ApiControllers.DockerControllers;

[ApiController, Route("api/docker")]
public class DockerController : YaginxControllerBase
{
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<DockerController> _logger;

    public DockerController(IDockerClient dockerClient, ILogger<DockerController> logger)
    {
        _dockerClient = dockerClient;
        _logger = logger;
    }
}

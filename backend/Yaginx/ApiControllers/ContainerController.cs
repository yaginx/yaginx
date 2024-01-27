using Docker.DotNet;
using Docker.DotNet.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Yaginx.Models;
using Yaginx.Services.DockerServices;

namespace Yaginx.Controllers;

[ApiController, Route("api/docker/container")]
public class ContainerController : Controller
{
	private readonly IDockerClient _dockerClient;
	private readonly ILogger<ContainerController> _logger;

	public ContainerController(IDockerClient dockerClient, ILogger<ContainerController> logger)
	{
		_dockerClient = dockerClient;
		_logger = logger;
	}

	[HttpGet, Route("list")]
	public async Task<IList<ContainerListResponse>> List()
	{
		IList<ContainerListResponse> containers = await _dockerClient.Containers.ListContainersAsync(
			new ContainersListParameters()
			{
				Limit = int.MaxValue,
				//Filters = "id=test&test=sdf"
			});

		return containers;
	}
	[HttpGet, Route("get")]
	public async Task<ContainerListResponse> Detail(string id)
	{
		ContainerListResponse? image = await GetContainerByContainerId(HttpUtility.UrlDecode(id));
		if (image == null)
			throw new Exception("Not Found");

		return image;
	}

	[AllowAnonymous]
	[HttpGet, Route("sample")]
	public async Task<IActionResult> GetSample()
	{
		var newContainerJson = new ReplaceNewImageRequest();

		newContainerJson.Name = "redis";
		newContainerJson.Image = "redis:latest";
		newContainerJson.Envs = new List<string>() { "Environment=Production" };
		newContainerJson.Volumns = new List<string>() { "/data:/data" };
		newContainerJson.Ports = new List<KeyValuePair<string, List<string>>>();
		newContainerJson.Ports.Add(new KeyValuePair<string, List<string>>("6379", new List<string> { "6379" }));
		return Json(newContainerJson);
	}

	[HttpPost, Route("replace")]
	public async Task<IActionResult> ReplaceImage([FromBody] ReplaceNewImageRequest request)
	{
		var queuedJobId = BackgroundJob.Enqueue<ContainerServcie>(service => service.ReplaceImage(request, null, default));
		await Task.CompletedTask;
		return Json(new { message = "queued", jobId = queuedJobId });
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

using AgileLabs;
using Docker.DotNet;
using Docker.DotNet.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Newtonsoft.Json;
using Yaginx.Configures.Dockers;
using Yaginx.Infrastructure.Configuration;
using Yaginx.Models;

namespace Yaginx.Services.DockerServices
{
    public class ContainerServcie
    {
        private readonly IDockerClient _dockerClient;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public ContainerServcie(IDockerClient dockerClient, AppSettings appSettings, ILogger<ContainerServcie> logger)
        {
            _dockerClient = dockerClient;
            _appSettings = appSettings;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ReplaceImage(ReplaceNewImageRequest request, PerformContext context, CancellationToken cancellationToken = default)
        {
            context?.WriteLine("Start Replace Image Progress");

            // 获取Image
            if (!string.IsNullOrEmpty(request.Image))
            {
                var authConfig = GetAuthConfig(request.Image);
                try
                {
                    await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = request.Image },
                                                                authConfig,
                                                                new Progress<JSONMessage>((message) => context?.WriteLine(JsonConvert.SerializeObject(message, Formatting.Indented))),
                                                                cancellationToken);

                    var imageInspectResult = await _dockerClient.Images.InspectImageAsync(request.Image);
                    if (imageInspectResult == null || string.IsNullOrEmpty(imageInspectResult.ID))
                    {
                        throw new Exception($"Image: {request.Image}不存在");
                    }

                    context?.WriteLine("Image 信息");
                    context?.WriteLine(JsonConvert.SerializeObject(imageInspectResult, Formatting.Indented));
                }
                catch (DockerImageNotFoundException ex)
                {
                    context?.WriteLine(ex.FullMessage());
                    throw;
                }
            }
            else
            {
                context?.WriteLine("Image未指定");
                throw new Exception($"Image未指定");
            }

            // 检查container是否存在, 如果存在删除
            ContainerListResponse? container = await GetContainerByContainerName(request.Name);

            // 开始创建容器
            var createContainerParameters = new CreateContainerParameters
            {
                Image = request.Image,
                Name = container?.Names?.FirstOrDefault() ?? request.Name,
                Tty = true,
                OpenStdin = true,
                HostConfig = new HostConfig { RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always } }
            };

            if (request.Ports != null && request.Ports.Any())
            {
                IDictionary<string, IList<PortBinding>> portBindings = new Dictionary<string, IList<PortBinding>>();
                foreach (var item in request.Ports)
                {
                    portBindings.Add(item.Key, item.Value.Select(x => new PortBinding { HostPort = x }).ToList());
                }
                createContainerParameters.HostConfig.PortBindings = portBindings;
            }

            if (request.Volumns != null && request.Volumns.Any())
            {
                if (createContainerParameters.HostConfig.Binds == null)
                    createContainerParameters.HostConfig.Binds = new List<string>();
                foreach (var item in request.Volumns)
                {
                    createContainerParameters.HostConfig.Binds.Add(item);
                }
            }

            if (request.Links != null && request.Links.Any())
            {
                if (createContainerParameters.HostConfig.Links == null)
                    createContainerParameters.HostConfig.Links = new List<string>();
                foreach (var item in request.Links)
                {
                    createContainerParameters.HostConfig.Links.Add(item);
                }
            }

            if (request.Envs != null && request.Envs.Any())
            {
                if (createContainerParameters.Env == null)
                    createContainerParameters.Env = new List<string>();
                foreach (var item in request.Envs)
                {
                    createContainerParameters.Env.Add(item);
                }
            }
            // 增加host支持
            //--add-host yourdomain.com:127.0.0.1
            if (request.Hosts != null && request.Hosts.Any())
            {
                if (createContainerParameters.HostConfig.ExtraHosts == null)
                    createContainerParameters.HostConfig.ExtraHosts = new List<string>();
                foreach (var item in request.Hosts)
                {
                    createContainerParameters.HostConfig.ExtraHosts.Add(item);
                }
            }
            if (container != null)
            {
                context?.WriteLine("Start Stop Old Container");
                await _dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters() { WaitBeforeKillSeconds = 100 * 1000 });

                context?.WriteLine("Start Remove Old Container");
                await _dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { });
            }

            context?.WriteLine("Start CreateContainerAsync");
            var createResult = await _dockerClient.Containers.CreateContainerAsync(createContainerParameters);

            context?.WriteLine("Start StartContainerAsync");
            var startResult = await _dockerClient.Containers.StartContainerAsync(createResult.ID, new ContainerStartParameters() { });

            var processResult = JsonConvert.SerializeObject(new { CreateResult = createResult, StartResult = startResult }, Formatting.Indented);
            _logger.LogInformation($"{processResult}");
            context?.WriteLine(processResult);
        }

        private AuthConfig? GetAuthConfig(string image)
        {
            AuthConfig authConfig = null;
            var imageDomain = image.IndexOf('/') > 0 ? image.Substring(0, image.IndexOf('/')) : string.Empty;
            if (!string.IsNullOrEmpty(imageDomain) && imageDomain.IndexOf('.') > 0)
            {
                var registryItem = _appSettings.Get<RegistryConfig>().FirstOrDefault(x => x.ServerAddress == imageDomain);
                authConfig = new AuthConfig { ServerAddress = registryItem.ServerAddress, Username = registryItem.UserName, Password = registryItem.Password };
            }

            return authConfig;
        }

        private async Task<ContainerListResponse?> GetContainerByContainerName(string name)
        {
            IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>();
            filters.Add("name", new Dictionary<string, bool> { { $"{name}", true } });

            var images = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = filters,
            });

            return images.FirstOrDefault();
        }


    }
}

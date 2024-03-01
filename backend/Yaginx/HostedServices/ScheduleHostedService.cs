using AgileLabs;
using AgileLabs.WorkContexts.Extensions;

namespace Yaginx.HostedServices
{

    internal class ScheduleHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ScheduleHostedService(ILogger<ScheduleHostedService> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            using (var scope = AgileLabContexts.Context.CreateScopeWithWorkContext())
            {
                var initService = scope.Resolve<InitService>();
                try
                {
                    await initService.Init();
                }
                catch
                {
                }

            }

            if (_hostEnvironment.IsDevelopment())
            {
                return;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
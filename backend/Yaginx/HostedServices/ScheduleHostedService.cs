using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Hangfire;

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
            //RecurringJob.AddOrUpdate<ChatgptUserDailyUsageReportService>(nameof(ChatgptUserDailyUsageReportService), service => service.LastDayUsageReportAsync(), "0 8 * * *", new RecurringJobOptions
            //{
            //    TimeZone = TimeZoneInfo.Local
            //});


            ////RecurringJob.AddOrUpdate<UnactiveUserInviceComeBackService>(nameof(UnactiveUserInviceComeBackService), service => service.InviteAsync(), "0 12 * * *", new RecurringJobOptions
            ////{
            ////    TimeZone = TimeZoneInfo.Local
            ////});

            RecurringJob.AddOrUpdate<CodeAutoGenerateService>(nameof(CodeAutoGenerateService), service => service.UserCodeGenerateAsync(), "0 * * * *", new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });

            //RecurringJob.AddOrUpdate<DailyTokenSupplementBackService>(nameof(DailyTokenSupplementBackService), service => service.SupplementAsync(), "0 0 * * *", new RecurringJobOptions
            //{
            //    TimeZone = TimeZoneInfo.Local
            //});
            await Task.CompletedTask;
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
using AgileLabs;
using AgileLabs.WebApp.Hosting;
using AgileLabs.WorkContexts.Extensions;
using Hangfire;
using Yaginx.DomainModels;

namespace Yaginx.HostedServices
{
    public class CodeAutoGenerateService : IScoped
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebDomainRepository _webDomainRepository;

        public CodeAutoGenerateService(IServiceProvider serviceProvider, IWebDomainRepository webDomainRepository)
        {
            _serviceProvider = serviceProvider;
            _webDomainRepository = webDomainRepository;
        }

        public async Task UserCodeGenerateAsync()
        {

        }
    }
    public class HostServiceRegister : IServiceRegister
    {
        public int Order => 100;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddHostedService<ScheduleHostedService>();
        }
    }
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
            if (_hostEnvironment.IsDevelopment())
            {
                return;
            }

            using (var scope = AgileLabContexts.Context.CreateScopeWithWorkContext())
            {
                var initService = scope.Resolve<InitService>();
                await initService.Init();
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
using Yaginx.Services;

namespace Yaginx.HostedServices
{
    internal class ReportResourceServiceTask : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ResourceReportServcie _resourceReportServcie;

        public ReportResourceServiceTask(ILogger<ReportResourceServiceTask> logger, IHostEnvironment hostEnvironment, ResourceReportServcie resourceReportServcie)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _resourceReportServcie = resourceReportServcie;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _resourceReportServcie.HourlyCheckAsync();
                    await _resourceReportServcie.DailyCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "统计异常");
                }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
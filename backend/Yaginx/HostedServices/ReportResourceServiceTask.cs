using AgileLabs;
using AgileLabs.WorkContexts.Extensions;
using Yaginx.Services;

namespace Yaginx.HostedServices
{
    internal class ReportResourceServiceTask : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ReportResourceServiceTask(ILogger<ReportResourceServiceTask> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var checkTimeSpan = TimeSpan.FromSeconds(15);
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
                var _resourceReportService = scope.WorkContext.Resolve<ResourceReportServcie>();
                try
                {
                    var nowTime = DateTime.Now;

                    #region 分钟级
                    if (nowTime <= nowTime.AddMinutes(-nowTime.Minute).Add(checkTimeSpan * 2))
                    {
                        await _resourceReportService.HourlyCheckAsync(DateTime.Now.AddMinutes(-1));// 再次统计上一分钟的数据
                    }

                    await _resourceReportService.MinutelyCheckAsync(DateTime.Now);
                    #endregion

                    #region Houly
                    if (nowTime.Minute % 15 == 0)
                    {
                        if (nowTime <= nowTime.Date.AddHours(nowTime.Hour).Add(checkTimeSpan * 2))
                        {
                            await _resourceReportService.HourlyCheckAsync(DateTime.Now.AddHours(-1));// 再次统计上一小时的数据
                        }

                        await _resourceReportService.HourlyCheckAsync(DateTime.Now);
                    }
                    #endregion

                    #region Daily
                    if (nowTime.Hour % 2 == 0 && nowTime.Minute % 30 == 0)
                    {
                        if (nowTime <= nowTime.Date.Add(checkTimeSpan * 2))
                        {
                            await _resourceReportService.DailyCheckAsync(DateTime.Now.Date.AddDays(-1));// 再次统计上一小时的数据
                        }

                        await _resourceReportService.DailyCheckAsync(DateTime.Now.Date);
                    }
                    #endregion
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
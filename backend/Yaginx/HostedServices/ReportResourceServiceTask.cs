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
            var checkTimeSpan = TimeSpan.FromSeconds(15);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nowTime = DateTime.Now;

                    #region 分钟级
                    if (nowTime <= nowTime.AddMinutes(-nowTime.Minute).Add(checkTimeSpan * 2))
                    {
                        await _resourceReportServcie.HourlyCheckAsync(DateTime.Now.AddMinutes(-1));// 再次统计上一分钟的数据
                    }

                    await _resourceReportServcie.MinutelyCheckAsync(DateTime.Now);
                    #endregion


                    #region Houly
                    if (nowTime <= nowTime.Date.AddHours(nowTime.Hour).Add(checkTimeSpan * 2))
                    {
                        await _resourceReportServcie.HourlyCheckAsync(DateTime.Now.AddHours(-1));// 再次统计上一小时的数据
                    }

                    await _resourceReportServcie.HourlyCheckAsync(DateTime.Now);

                    #endregion

                    #region Daily
                    if (nowTime <= nowTime.Date.Add(checkTimeSpan * 2))
                    {
                        await _resourceReportServcie.DailyCheckAsync(DateTime.Now.Date.AddDays(-1));// 再次统计上一小时的数据
                    }

                    await _resourceReportServcie.DailyCheckAsync(DateTime.Now.Date);

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
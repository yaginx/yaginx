using AgileLabs;
using AgileLabs.MemoryBuses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Yaginx.Infrastructure;


/// <summary>
/// 请求统计中间件
/// </summary>
public class TrafficMonitorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryBus _memoryBus;
    private ConcurrentQueue<MonitorRawInfo> _perSecondStore = new ConcurrentQueue<MonitorRawInfo>();

    public TrafficMonitorMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        IHostApplicationLifetime hostApplicationLifetime, IHostEnvironment hostEnvironment,
        IMemoryBus memoryBus
        )
    {
        if (loggerFactory is null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _next = next ?? throw new ArgumentNullException(nameof(next));

        _logger = loggerFactory.CreateLogger("EMWS");
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        _hostEnvironment = hostEnvironment;
        _memoryBus = memoryBus ?? throw new ArgumentNullException(nameof(memoryBus));

        //Task.Factory.StartNew(async () => await CreateRemoteConnection(_hostApplicationLifetime.ApplicationStopping), hostApplicationLifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        Task.Factory.StartNew(async () => await StaticitsSecondDataTask(_hostApplicationLifetime.ApplicationStopping), hostApplicationLifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //Task.Factory.StartNew(async () => await StaticitsBufferClearTask(_hostApplicationLifetime.ApplicationStopping), hostApplicationLifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //Task.Factory.StartNew(async () => await PushData(_hostApplicationLifetime.ApplicationStopping), hostApplicationLifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// 统计每秒的数据
    /// </summary>
    private async Task StaticitsSecondDataTask(CancellationToken applicationStopping)
    {
        _logger.LogDebug("StaticitsSecondData Task Started");
        while (!applicationStopping.IsCancellationRequested && _hostEnvironment.IsProduction())
        {
            var lastPeriodData = Interlocked.Exchange(ref _perSecondStore, new ConcurrentQueue<MonitorRawInfo>());
            //_messageStore.Enqueue(new MonitorMessage() { ts = DateTime.Now.GetEpochMilliseconds(), data = lastPeriodData.ToList() });
            try
            {
                if (lastPeriodData.Any())
                    await _memoryBus.SendAsync(new MonitorMessage() { ts = DateTime.Now.GetEpochSeconds(), data = lastPeriodData.ToList() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "秒级统计异常");
            }
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        _logger.LogDebug("StaticitsSecondData Task End");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var durationSw = Stopwatch.StartNew();
        var request = context.Request;
        MonitorRawInfo monitorInfo = new MonitorRawInfo()
        {
            Host = request.Host.Value,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Lang = request.Headers.GetHeaderValueAs<string>("Accept-Language"),
            Referer = request.Headers.GetHeaderValueAs<string>("Referer"),
            UserAgent = request.Headers.GetHeaderValueAs<string>("User-Agent"),
            Ip = context.GetClientIp()
        };
        await _next(context);

        monitorInfo.StatusCode = context.Response.StatusCode;
        monitorInfo.Duration = durationSw.ElapsedMilliseconds;
        durationSw.Stop();
        _perSecondStore.Enqueue(monitorInfo);
    }
}
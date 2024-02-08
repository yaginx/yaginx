using AgileLabs;
using AgileLabs.Diagnostics;
using AgileLabs.FileProviders;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Yaginx.DomainModels;

namespace Yaginx.Infrastructure
{
    public class DomainTrafficMiddleware
    {
        public RequestDelegate Next { get; }
        public const string STATS_PATH = "/traffic";
        private readonly IEmbeddedResourceQuery _embeddedResourceQuery;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// 处理中请求地址Cache
        /// </summary>
        private ConcurrentDictionary<string, DomainTraffic> _requestTraffic = new ConcurrentDictionary<string, DomainTraffic>();

        public DomainTrafficMiddleware(RequestDelegate next, IEmbeddedResourceQuery embeddedResourceQuery, IHostApplicationLifetime hostApplicationLifetime)
        {
            Next = next;
            _embeddedResourceQuery = embeddedResourceQuery;
            _hostApplicationLifetime = hostApplicationLifetime;
            Task.Factory.StartNew(FlushTask);
        }

        public async Task Invoke(HttpContext context)
        {
            var requestPath = context.Request.Path.ToString();

            //开始请求处理计时器
            try
            {
                var domain = context.Request.Host.Host;
                if (!_requestTraffic.ContainsKey(domain))
                {
                    _requestTraffic.TryAdd(domain, new DomainTraffic());
                }

                var domainTraffic = _requestTraffic[domain];
                domainTraffic.Requests += 1;
                domainTraffic.Inbound += context.Request.ContentLength ?? 0;
                _requestTraffic[domain] = domainTraffic;

                await Next(context);

                domainTraffic.Outbound += context.Response.ContentLength ?? 0;
                _requestTraffic[domain] = domainTraffic;

                if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith(STATS_PATH, StringComparison.OrdinalIgnoreCase))
                {
                    await ProcessStatusPage(context);
                }
            }
            finally
            {
                //_requestPathCache.TryRemove(context.Connection.Id, out _);
                //EnqueueRequest(stopwatch);
                //stopwatch.Stop();
                //Interlocked.Decrement(ref _runningRequestCounter);
            }
        }

        private async Task ProcessStatusPage(HttpContext context)
        {
            var fileStream = _embeddedResourceQuery.Read(typeof(Program).Assembly, "EmbeddedViews.StatusPage.html");
            if (fileStream == null)
                return;

            using (var streamReader = new StreamReader(fileStream))
            {
                var fileContent = await streamReader.ReadToEndAsync();
                fileContent = fileContent.Replace("{HOST}", $"{context.Request.Scheme}://{context.Request.Host}");

                var viewData = new
                {
                    message = "Hello NodeGateway",
                    hostName = Environment.MachineName,
                    version = VersionTools.GetVersionString(),
                    domainTraffics = new List<dynamic>()
                };

                var domainTraffices = _requestTraffic.Select(x => new
                {
                    domain = x.Key,
                    requests = x.Value.Requests,
                    inbound = x.Value.Inbound,
                    outbound = x.Value.Outbound
                }).ToList<dynamic>();
                viewData.domainTraffics.AddRange(domainTraffices);

                fileContent = fileContent.Replace("'VIEWJSON'", JsonConvert.SerializeObject(viewData));
                await context.Response.Body.WriteContent(fileContent);
            }
        }

        private async Task FlushTask()
        {
            var flushPeriodInSeconds = 5;
            while (true)
            {
                if (_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(flushPeriodInSeconds));
                var lastPeriodData = Interlocked.Exchange(ref _requestTraffic, new ConcurrentDictionary<string, DomainTraffic>());

                if (!lastPeriodData.Any())
                {
                    continue;
                }

                using var scope = AgileLabContexts.Context.CreateScopeWithWorkContext();
                var trafficRepository = scope.WorkContext.ServiceProvider.GetRequiredService<IHostTrafficRepository>();
                foreach (var item in lastPeriodData)
                {
                    trafficRepository.Upsert(new HostTraffic
                    {
                        Id = IdGenerator.NextId(),
                        HostName = item.Key,
                        InboundBytes = item.Value.Inbound,
                        OutboundBytes = item.Value.Outbound,
                        RequestCounts = item.Value.Requests,
                    });
                }
            }
        }
    }
}
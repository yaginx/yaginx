using AgileLabs;
using AgileLabs.Diagnostics;
using AgileLabs.FileProviders;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Yaginx.Infrastructure
{
    public class DomainTrafficMiddleware
	{
		public RequestDelegate Next { get; }
		public const string STATS_PATH = "/traffic";
		private readonly IEmbeddedResourceQuery _embeddedResourceQuery;

		/// <summary>
		/// 处理中请求地址Cache
		/// </summary>
		private ConcurrentDictionary<string, DomainTraffic> _requestTraffic = new ConcurrentDictionary<string, DomainTraffic>();

		public DomainTrafficMiddleware(RequestDelegate next, IEmbeddedResourceQuery embeddedResourceQuery)
		{
			Next = next;
			_embeddedResourceQuery = embeddedResourceQuery;
		}

		public async Task Invoke(HttpContext context)
		{
			var requestPath = context.Request.Path.ToString();

			//开始请求处理计时器
			try
			{
				if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith(STATS_PATH, StringComparison.OrdinalIgnoreCase))
				{
					await ProcessStatusPage(context);
				}
				else
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
	}
}

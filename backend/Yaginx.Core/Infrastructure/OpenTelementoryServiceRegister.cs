using AgileLabs.Diagnostics;
using AgileLabs.Sessions;
using AgileLabs.WebApp.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Yaginx.Infrastructure;

public static class OpenTelementoryServiceRegister
{

	public static void ConfigureOpenTelementoryService(this IServiceCollection services, AppBuildContext buildContext)
	{
		if (buildContext.HostEnvironment.EnvironmentName.Contains("test", StringComparison.OrdinalIgnoreCase))
		{
			// 如果是测试环境, 不增加APM检测
			return;
		}

		services.Configure<OtlpExporterOptions>(options =>
		{
			buildContext.Configuration.GetSection("OpenTelemetry:Collector").Bind(options);
		});
		services.AddOpenTelemetry()
			.ConfigureResource(resource =>
			{
				resource.AddService(
					serviceNamespace: "yaginx",
					serviceName: buildContext.HostEnvironment.ApplicationName,
					serviceVersion: VersionTools.GetVersionString(),
					serviceInstanceId: Environment.MachineName
				);
				resource.AddAttributes(new Dictionary<string, object>
				{
					{ "deployment.environment", buildContext.HostEnvironment.EnvironmentName }
				});
			})
			.WithTracing(tracing => tracing
				.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(buildContext.HostEnvironment.ApplicationName))
				.AddAspNetCoreInstrumentation(opt =>
				{
					opt.EnrichWithHttpRequest = (activity, httpRequest) =>
					{
						var requestSession = httpRequest.HttpContext.RequestServices.GetRequiredService<IRequestSession>();
						activity.SetBaggage("ClientIp", requestSession.ClientIp);
					};
				})
				.AddEntityFrameworkCoreInstrumentation()
				.AddSqlClientInstrumentation()
				.AddHttpClientInstrumentation()
				.AddRedisInstrumentation()
				//.AddHangfireInstrumentation()
				//.AddRedisInstrumentation()
				.AddOtlpExporter()
				//.AddConsoleExporter()
				)
			.WithMetrics(metrics => metrics
				.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(buildContext.HostEnvironment.ApplicationName))
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				//.AddRuntimeInstrumentation()
				//.AddProcessInstrumentation()
				//.AddRuntimeInstrumentation()
				//.AddEventCountersInstrumentation()
				.AddView(
					instrumentName: "orders-price",
					new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 15, 30, 45, 60, 75 } })
				.AddView(
					instrumentName: "orders-number-of-books",
					new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 1, 2, 5 } })
				.AddOtlpExporter()
				//.AddConsoleExporter()
				);
	}
}

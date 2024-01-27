using AgileLabs.WebApp;
using AgileLabs.WebApp.Hosting;
using Yaginx.Configures;

public class YaginxApplicationOptions : DefaultMvcApplicationOptions
{
	public YaginxApplicationOptions()
	{
		//UseSerilogProvider = false;
		TypeFinderAssemblyScanPattern = "^Yaginx|^AgileLabs";
		//MvcBuilderCreateFunc = (IServiceCollection serviceCollection, Action<MvcOptions> action) => serviceCollection.add(action);

		// 引入appsettings.global.json文件
		// 现阶段所有应用部署在一台服务器上, 共享同样的目录/data/yaginx
		ConfigureWebApplicationBuilder += (WebApplicationBuilder builder, AppBuildContext buildContext) =>
		{
			builder.Configuration.AddJsonFile(ConfigurationDefaults.AppSettingsFilePath, true, true);
			if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
			{
				var path = string.Format(ConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
				builder.Configuration.AddJsonFile(path, true, true);
			}

			builder.Configuration.AddJsonFile($"ReverseProxyConfig.json", optional: true, reloadOnChange: true);

			if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
			{
				builder.Configuration.AddJsonFile($"ReverseProxyConfig.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
			}
		};

		//ConfigureLoggingBuilder += (ILoggingBuilder loggingBuilder, AppBuildContext context) =>
		//{
		//	loggingBuilder.ClearProviders();
		//	loggingBuilder.Configure(options =>
		//	{
		//		options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId | ActivityTrackingOptions.ParentId | ActivityTrackingOptions.Baggage | ActivityTrackingOptions.Tags;
		//	});
		//};
	}
}

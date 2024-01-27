using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.AspNet.ClientAppServices;
using AgileLabs.AspNet.WebApis.Filters;
using AgileLabs.Json;
using AgileLabs.WebApp.Hosting;
using Docker.DotNet;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Yaginx.Configures.Hangfires;
using Yaginx.Infrastructure;
using Yaginx.Infrastructure.Configuration;
using Yaginx.Infrastructure.Securities;
using Yaginx.Services.Securities;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

public class YaginxAppConfigure : IServiceRegister, IRequestPiplineRegister, IEndpointConfig, IMvcOptionsConfig, IMvcBuildConfig
{
	public int Order => 1;
	public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
	{
		services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "app_data")));

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		// Add the reverse proxy capability to the server
		#region ReverseProxy
		services.AddReverseProxy()
			.LoadFromConfig(buildContext.Configuration.GetSection("ReverseProxy"))
			.AddTransforms(builderContext =>
			{
				/*
				 Append: 在上游的基础上带上当前节点信息传给下游
				 Set: 忽略上游信息, 使用当前节点信息往下传
				 Off:直接把上游信息传给下游,忽略当前节点
				 Remove: 不往下传该信息
				 */
				builderContext.AddXForwardedFor(action: ForwardedTransformActions.Append);// 在上游的基础上,增加当前节点信息传给下游
				builderContext.AddXForwardedHost(action: ForwardedTransformActions.Append);// 在上游的基础上,增加当前节点信息传给下游
				builderContext.AddXForwardedProto(action: ForwardedTransformActions.Append);// 在上游的基础上,增加当前节点信息传给下游
				builderContext.AddXForwardedPrefix(action: ForwardedTransformActions.Append);
			})
			.AddTransforms(DefaultRouteTransform);
		#endregion

		#region DockerEgnine
		services.AddSingleton<IDockerClient>(serviceProvider =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var conn = configuration.GetSection("DockerEngine:Socket").Value;
			DockerClient client = new DockerClientConfiguration(new Uri(conn)).CreateClient();
			return client;
		});
		#endregion

		#region Settings
		//create default file provider
		CommonHelper.DefaultFileProvider = new AppFileProvider(buildContext.HostEnvironment);

		var typeFinder = buildContext.TypeFinder;
		//add configuration parameters
		var configurations = typeFinder
			.FindClassesOfType<IConfig>()
			.Select(configType => (IConfig)Activator.CreateInstance(configType))
			.ToList();

		foreach (var config in configurations)
		{
			buildContext.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);
		}
		var appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, false);
		services.AddSingleton(appSettings);
		#endregion

		#region Hangfire
		// Add Hangfire services.
		services.AddHangfire((provider, configuration) =>
		{
			configuration
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseSerilogLogProvider()
			   .UseConsole()
			   .UseMemoryStorage();
		});

		// Add the processing server as IHostedService
		services.AddSingleton<HangfireJobActivator>();
		services.AddSingleton<JobActivator>(serficeProvider => serficeProvider.GetRequiredService<HangfireJobActivator>());
		services.AddHangfireServer();

		#endregion

		services.AddClientApp(options =>
		{
			options.RootPath = "ClientApp";

			var adminClientApp = ClientAppConfig.Create("/adminui", "AdminUI");
			adminClientApp.DefaultPageStaticFileOptions.OnPrepareResponse = ctx =>
			{
				// Cache static files for 12 hours
				ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=60, immutable");
				ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddMinutes(1).ToString("R", CultureInfo.InvariantCulture));
			};

			options.ClientApps.Add(adminClientApp);
		});

		#region Authentication & Authorization
		services.AddSingleton<IAuthenticateService, AuthenticateService>();
		services.ConfigureSecurityServices(buildContext);
		#endregion

		services.ConfigureOpenTelementoryService(buildContext);
	}

	public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
	{
		piplineActions.Register("Static Resource", RequestPiplineStage.BeforeRouting, app =>
		{
			app.UseDeveloperExceptionPage();

			if (!buildContext.HostEnvironment.IsDevelopment())
			{
				app.UseResponseCompression();
			}

			app.UseResponseCaching();

			app.UseClientApp();

			app.UseStaticFiles(new StaticFileOptions
			{
				OnPrepareResponse = ctx =>
				{
					// Cache static files for 12 hours
					ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000, immutable");
					ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddHours(12).ToString("R", CultureInfo.InvariantCulture));
				}
			});
		});

		piplineActions.Register("OpenApi Docs", RequestPiplineStage.BeforeRouting, app =>
		{
			if (buildContext.HostEnvironment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
		});

		piplineActions.Register("HangfireDashboard", RequestPiplineStage.BeforeRouting, app =>
		{
			app.UseHangfireDashboard(options: new DashboardOptions()
			{
				DashboardTitle = "Jobs",
				Authorization = new[] { new HangfireAuthorizationFilter() }
			});
		});

		piplineActions.Register("Security", RequestPiplineStage.BeforeEndpointConfig, app =>
		{
			if (buildContext.HostEnvironment.IsDevelopment())
			{
				app.UseAuthentication();
				app.UseAuthorization();
			}
		});

		return piplineActions;
	}

	public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, AppBuildContext appBuildContext)
	{
		endpoints.MapReverseProxy();
		endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}");
		//endpoints.MapFallbackToFile(DefaultIndexFileName);
	}

	public static void DefaultRouteTransform(TransformBuilderContext builderContext)
	{
		// 转发OriginalHost
		builderContext.AddOriginalHost(true);

		// 往下游转发时附加认证信息
		builderContext.AddRequestTransform(async requestTransformContext =>
		{
			// 这里加入当前的认证信息
			//var identityInfo = requestTransformContext.HttpContext.User.Identity as ClaimsIdentity;
			//if (identityInfo.IsAuthenticated)
			//{
			//	var ssid = identityInfo.GetClaimValueAs<string>(ClaimNames.Ssid);
			//	requestTransformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("SessionKey", ssid);
			//}
			await Task.CompletedTask;
		});
	}

	public void ConfigureMvcOptions(MvcOptions mvcOptions, AppBuildContext appBuildContext)
	{
		mvcOptions.Filters.Add<ExceptionHandlerFilter>();
		mvcOptions.Filters.Add<EnvelopFilterAttribute>();
	}

	public void ConfigureMvcBuilder(IMvcBuilder mvcBuilder, AppBuildContext appBuildContext)
	{
		// Newtonsoft.Json
		// 全局Json序列化的配置
		// 放这里是因为网关也需要
		mvcBuilder.AddNewtonsoftJson(jsonOptions =>
		{
			JsonNetSerializerSettings.DeconretCamelCaseSerializerSettings(jsonOptions.SerializerSettings);
			//jsonOptions.SerializerSettings.Converters.Add(LongStringJsonConverter.Instance);
			//jsonOptions.SerializerSettings.Converters.Add(BoolIntJsonConverter.Instance);
			//jsonOptions.SerializerSettings.Converters.Add(DateTimeToTimestampJsonConverter.Instance);
			//jsonOptions.SerializerSettings.Converters.Add(NullableDateTimeToTimestampJsonConverter.Instance);
			JsonNetSerializerSettings.Instance = jsonOptions.SerializerSettings;
		});
	}
}

using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.AspNet;
using AgileLabs.AspNet.ClientAppServices;
using AgileLabs.AspNet.WebApis.Filters;
using AgileLabs.Diagnostics;
using AgileLabs.FileProviders;
using AgileLabs.Json;
using AgileLabs.Json.Converters;
using AgileLabs.Securities;
using AgileLabs.WebApp.Hosting;
using AgileLabs.WorkContexts;
using Docker.DotNet;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using LettuceEncrypt;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Yaginx.Configures.Hangfires;
using Yaginx.DataStore.MongoStore;
using Yaginx.HostedServices;
using Yaginx.Infrastructure;
using Yaginx.Infrastructure.Configuration;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.Infrastructure.Securities;
using Yaginx.MemoryBuses;
using Yaginx.Services;
using Yaginx.Services.DockerServices;
using Yaginx.Services.Securities;
using Yaginx.WorkContexts;
using Yaginx.YaginxAcmeLoaders;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;
public class YaginxAppConfigure : IServiceRegister, IRequestPiplineRegister, IEndpointConfig, IMvcOptionsConfig, IMvcBuildConfig
{
    public int Order => 1;
    public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
    {
        services.AddNiusysSecurity(options =>
        {
            options.EncryptionKey = "2218EF6E-7D95-442F-B967-3979B00E9226";
        });
        services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
        services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });

        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(AppData.GetPath("DataProtectionKeys")));

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // https://stackoverflow.com/questions/55289631/inconsistent-behaviour-with-modelstate-validation-asp-net-core-api
            options.SuppressModelStateInvalidFilter = true;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddResponseCaching();
        services.AddResponseCompression();

        services.AddCustomWorkContext<IWorkContext, IWorkContextSetter, DefaultWorkContextFactory>();

        // Add the reverse proxy capability to the server
        #region ReverseProxy
        services.AddScoped<ProxyRuleRedisStorageService>();
        services.AddSingleton<ProxyRuleChangeNotifyService>();
        services.AddHostedService<ProxyRuleChangeReceiveService>();

        services.AddSingleton<IProxyConfigProvider, FileProxyConfigProvider>();
        services.AddHttpForwarder();
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
            .FindClassesOfType<IFileConfig>()
            .Select(configType => (IFileConfig)Activator.CreateInstance(configType))
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
            options.ClientApps.Add(ClientAppConfig.Create("/adminui", "AdminUI"));
            options.ClientApps.Add(ClientAppConfig.Create("/helps", "HelpDocs"));
        });

        #region Authentication & Authorization
        services.AddSingleton<IAuthenticateService, AuthenticateService>();
        services.ConfigureSecurityServices(buildContext);
        #endregion

        services.ConfigureOpenTelementoryService(buildContext);

        #region Certificates
        services.AddLettuceEncrypt().PersistDataToDirectory(new DirectoryInfo(AppData.Path), string.Empty);
        services.AddScoped<YaginxAcmeCertificateFactory>()
            .AddScoped<YaginxBeginCertificateCreationState>()
            .AddScoped<YaginxServerStartupState>()
            .AddScoped<YaginxTlsAlpnChallengeResponder>()
            .AddScoped<YaginxTlsAlpn01DomainValidator>()
            .AddScoped<YaginxCheckForRenewalState>();

        services.AddHostedService<YaginxAcmeCertificateLoader>();
        #endregion


        services.UseLiteDBDataStore(buildContext);

        services.AddHostedService<ScheduleHostedService>();

        services.AddScoped<ContainerServcie>();

        #region Resource Monitor
        services.AddScoped<ResourceReportServcie>();
        services.AddScoped<TrafficMonitorInfoEventSubscriber>();
        services.AddHostedService<ReportResourceServiceTask>();
        services.AddMemoryBus();

        services.RegisterMongo(options =>
        {
            buildContext.Configuration.GetSection("MongoSetting:Default").Bind(options);
        });
        #endregion
    }

    public const string BasePath = "/yaginx";
    public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
    {
        piplineActions.Register("UseBasePath", RequestPiplineStage.BeforeRouting, app => app.UsePathBase(BasePath));
        piplineActions.Register("TrafficMonitorMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<TrafficMonitorMiddleware>());
        piplineActions.Register("DomainTrafficMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<DomainTrafficMiddleware>());
        piplineActions.Register("ProcessDurationMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<TraceInfoHeaderOutputMiddleware>());
        piplineActions.Register("RequestStatisticsMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<RequestStatisticsMiddleware>());
        piplineActions.Register("MapDiagnositicRequest", RequestPiplineStage.BeforeRouting, app => app.MapDiagnositicRequest());
        piplineActions.Register("DiagnositicMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<DiagnositicMiddleware>());

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
            app.UseSwagger(optons =>
            {
                optons.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{BasePath}" } };
                });
            });
            app.UseSwaggerUI();
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
            app.UseAuthentication();
            app.UseAuthorization();
        });

        return piplineActions;
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, AppBuildContext appBuildContext)
    {
        endpoints.MapReverseProxy();
        //endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}");

        endpoints.MapDefaultControllerRoute();
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
            jsonOptions.SerializerSettings.Converters.Add(LongStringJsonConverter.Instance);
            jsonOptions.SerializerSettings.Converters.Add(NullableLongStringJsonConverter.Instance);
            //jsonOptions.SerializerSettings.Converters.Add(BoolIntJsonConverter.Instance);
            jsonOptions.SerializerSettings.Converters.Add(DateTimeToTimestampJsonConverter.Instance);
            jsonOptions.SerializerSettings.Converters.Add(NullableDateTimeToTimestampJsonConverter.Instance);
            JsonNetSerializerSettings.Instance = jsonOptions.SerializerSettings;
        });
    }
}


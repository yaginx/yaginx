using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.AspNet;
using AgileLabs.AspNet.ClientAppServices;
using AgileLabs.AspNet.WebApis.Filters;
using AgileLabs.FileProviders;
using AgileLabs.Json;
using AgileLabs.Json.Converters;
using AgileLabs.MemoryBuses;
using AgileLabs.Securities;
using AgileLabs.WebApp.Hosting;
using AgileLabs.WorkContexts;
using Docker.DotNet;
using Google.Protobuf.WellKnownTypes;
using LettuceEncrypt;
using LettuceEncrypt.Internal;
using LettuceEncrypt.YaginxAcmeLoaders;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using Scintillating.ProxyProtocol.Middleware;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Yaginx;
using Yaginx.DataStore.PostgreSQLStore;
using Yaginx.HostedServices;
using Yaginx.Infrastructure;
using Yaginx.Infrastructure.Configuration;
using Yaginx.Infrastructure.ProxyConfigProviders;
using Yaginx.Infrastructure.Securities;
using Yaginx.Services;
using Yaginx.Services.DockerServices;
using Yaginx.Services.Securities;
using Yaginx.SimpleProcessors;
using Yaginx.SimpleProcessors.ConfigProviders;
using Yaginx.WorkContexts;
using Yaginx.YaginxAcmeLoaders;
using Yarp.ReverseProxy.Configuration;
public partial class YaginxAppConfigure : IServiceRegister, IRequestPiplineRegister, IEndpointConfig, IMvcOptionsConfig, IMvcBuildConfig
{
    public int Order => 1;
    public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
    {
        buildContext.Items.Add("AppConfigure", this);
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

        if (buildContext.HostEnvironment.IsDevelopment())
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        services.AddResponseCaching();
        services.AddResponseCompression();

        services.AddCustomWorkContext<IWorkContext, IWorkContextSetter, DefaultWorkContextFactory>();

        var runningMode = RunningModes.RunningMode;
        if ((runningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            #region ReverseProxy
            services.AddScoped<ProxyRuleRedisStorageService>();
            services.AddSingleton<ProxyRuleChangeNotifyService>();
            services.AddHostedService<ProxyRuleChangeReceiveService>();

            services.AddSingleton<IProxyConfigProvider, FileProxyConfigProvider>();
            services.AddHttpForwarder();
            services.AddReverseProxy();
            #endregion
        }
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

        if ((runningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            services.AddClientApp(options =>
            {
                options.RootPath = "ClientApp";
                //options.PathBase = BasePath;
                options.ClientApps.Add(ClientAppConfig.Create($"{BasePath}/adminui", "AdminUI"));
                options.ClientApps.Add(ClientAppConfig.Create($"{BasePath}/helps", "HelpDocs"));
            });
        }

        #region Authentication & Authorization
        services.AddSingleton<IAuthenticateService, AuthenticateService>();
        services.ConfigureSecurityServices(buildContext);
        #endregion

        services.ConfigureOpenTelementoryService(buildContext);
        if ((runningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            services.AddSingleton<ISimpleProcessorConfigProvider, SimpleProcessorConfigProvider>();
            services.AddSimpleProcessor();

            #region Certificates
            services.AddLettuceEncrypt().PersistDataToDirectory(new DirectoryInfo(AppData.Path), string.Empty);
            services.AddScoped<YaginxAcmeCertificateFactory>()
                .AddScoped<YaginxBeginCertificateCreationState>()
                .AddScoped<YaginxServerStartupState>()
                .AddScoped<YaginxHttp01DomainValidator>()
                //.AddScoped<YaginxTlsAlpnChallengeResponder>()
                //.AddScoped<YaginxTlsAlpn01DomainValidator>()
                .AddScoped<YaginxCheckForRenewalState>()
                .AddSingleton<IHttpChallengeResponseStore, YaginxGlobalHttpChallengeResponseStore>();

            services.AddHostedService<YaginxAcmeCertificateLoader>();
            #endregion

            //services.UseLiteDBDataStore(buildContext);
            services.AddedPostgreSQLStore(buildContext);

            services.AddSingleton<ICertificateDomainRepsitory, CertificateDomainSingletonService>();

            services.AddHostedService<ScheduleHostedService>();
        }
        services.AddScoped<ContainerServcie>();

        if ((runningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            #region Resource Monitor
            services.AddScoped<ResourceReportServcie>();
            services.AddScoped<TrafficMonitorInfoEventSubscriber>();
            services.AddHostedService<ReportResourceServiceTask>();
            services.AddMemoryBus();
            #endregion
        }

        services.Configure((Action<MonitorInterafceOption>)(options =>
        {
            options.MatchPattern = buildContext.Configuration.GetSection("MonitorInterfacePattern").Value;
        }));
    }

    public const string BasePath = "/yaginx";
    public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
    {
        //piplineActions.Register("UseBasePath", RequestPiplineStage.BeforeRouting, app => app.UsePathBase(BasePath));
        if ((RunningModes.RunningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            piplineActions.Register("TrafficMonitorMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<TrafficMonitorMiddleware>());
            piplineActions.Register("DomainTrafficMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<DomainTrafficMiddleware>());
            piplineActions.Register("ProcessDurationMiddleware", RequestPiplineStage.BeforeRouting, app => app.UseMiddleware<TraceInfoHeaderOutputMiddleware>());

            piplineActions.Register("Static Resource", RequestPiplineStage.BeforeRouting, app =>
            {
                app.UseClientApp();
            });
            if (buildContext.HostEnvironment.IsDevelopment())
            {
                piplineActions.Register("OpenApi Docs", RequestPiplineStage.BeforeRouting, app =>
                {
                    app.UseSwagger(optons =>
                    {
                        optons.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                        {
                            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                        });
                    });
                    app.UseSwaggerUI(options => { });
                });
            }
        }

        piplineActions.Register("Security", RequestPiplineStage.BeforeEndpointConfig, app =>
        {
            app.UseAuthentication();
            app.UseAuthorization();
        });

        return piplineActions;
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, AppBuildContext appBuildContext)
    {
        var runningMode = RunningModes.RunningMode;
        if ((runningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode)
        {
            try
            {
                endpoints.MapSimpleProcessor(SimpleProcessorBuilder.ProxyBuilder);
                endpoints.MapReverseProxy(ReverseProxyBuilder.ProxyBuilder);
            }
            catch (Exception ex)
            {
                appBuildContext.BootstrapLogger.LogError(0, ex, "Yarp配置错误");
            }

        }
        endpoints.MapGet("/yaginx/ip", (HttpContext context) =>
        {
            var feature = context.Features.Get<IProxyProtocolFeature>();
            if (feature != null)
            {
                context.Response.Headers["X-Connection-Orignal-Remote-EndPoint"] = feature.OriginalRemoteEndPoint?.ToString();
                context.Response.Headers["X-Connection-Orignal-Local-EndPoint"] = feature.OriginalLocalEndPoint?.ToString();
            }

            context.Response.Headers["X-Request-Protocol"] = context.Request.Protocol;
            context.Response.Headers["X-Connection-Remote-IP"] = context.Connection.RemoteIpAddress?.ToString();
            context.Response.Headers["X-Connection-Remote-Port"] = context.Connection.RemotePort.ToString();
            context.Response.Headers["X-Connection-Local-IP"] = context.Connection.LocalIpAddress?.ToString();
            context.Response.Headers["X-Connection-Local-Port"] = context.Connection.LocalPort.ToString();

            context.Response.Headers["X-Request-IsHttps"] = context.Request.IsHttps.ToString();
            return new { ProxyProtocol = feature?.ToString() };
        });
        endpoints.MapDefaultControllerRoute();
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


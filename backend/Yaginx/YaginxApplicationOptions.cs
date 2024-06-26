using AgileLabs;
using AgileLabs.WebApp;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Scintillating.ProxyProtocol.Middleware;
using Yaginx;
using Yaginx.Configures;
public class YaginxApplicationOptions : DefaultMvcApplicationOptions
{
    public YaginxApplicationOptions()
    {
        TypeFinderAssemblyScanPattern = "^Yaginx|^AgileLabs";

        // 引入appsettings.global.json文件
        // 现阶段所有应用部署在一台服务器上, 共享同样的目录/data/yaginx
        ConfigureWebApplicationBuilder += (WebApplicationBuilder builder, AppBuildContext buildContext) =>
        {
            RunningMode runningMode = RunningMode.All;
            var runningModeEnv = Environment.GetEnvironmentVariable(RunningModes.Key);
            if (runningModeEnv.IsNotNullOrWhitespace())
            {
                switch (runningModeEnv.ToUpper())
                {
                    case "GW":
                    case "GATEWAY":
                        runningMode = RunningMode.GatewayMode;
                        break;
                    case "DP":
                    case "DOCKERPANEL":
                        runningMode = RunningMode.DockerPanelMode;
                        break;
                    default:
                        break;
                }
            }

            buildContext.Items.Add("RUNNING_MODE", runningMode);

            builder.Configuration.AddJsonFile(AppData.GetPath(ConfigurationDefaults.AppSettingsFilePath), true, true);
            if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
            {
                var path = string.Format(ConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
                builder.Configuration.AddJsonFile(AppData.GetPath(path), true, true);
            }
        };

        ConfigureWebHostBuilder += (IWebHostBuilder webHostBuilder, AppBuildContext buildContext) =>
        {
            webHostBuilder.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Limits.MaxRequestBodySize = null;
                serverOptions.Limits.MaxRequestBufferSize = 5 * 1024 * 1024;//5M

                serverOptions.ListenAnyIP(8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                // With Proxy Protocol
                serverOptions.ListenAnyIP(9080, listenOptions =>
                {
                    listenOptions.UseProxyProtocol(proxyProtocolOptions =>
                    {
                        context.Configuration.GetSection("ProxyProtocol").Bind(proxyProtocolOptions);
                    });
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                //// enables PROXY protocol for all endpoints
                //serverOptions.ConfigureEndpointDefaults(listenOptions =>
                //{
                //    listenOptions.UseProxyProtocol();
                //});

                if (((RunningModes.RunningMode & RunningMode.GatewayMode) == RunningMode.GatewayMode))
                {
                    serverOptions.ListenAnyIP(8443, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        listenOptions.UseHttps(httpsOptions =>
                        {
                            httpsOptions.UseYaginxLettuceEncrypt(serverOptions.ApplicationServices);
                        });
                    });

                    // With Proxy Protocol
                    serverOptions.ListenAnyIP(9443, listenOptions =>
                    {
                        listenOptions.UseProxyProtocol(proxyProtocolOptions =>
                        {
                            context.Configuration.GetSection("ProxyProtocol").Bind(proxyProtocolOptions);
                        });
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        listenOptions.UseHttps(httpsOptions =>
                        {
                            httpsOptions.UseYaginxLettuceEncrypt(serverOptions.ApplicationServices);
                        });
                    });
                }
            });
        };
    }
}

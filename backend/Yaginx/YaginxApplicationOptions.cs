using AgileLabs.WebApp;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
            builder.Configuration.AddJsonFile(AppData.GetPath(ConfigurationDefaults.AppSettingsFilePath), true, true);
            if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
            {
                var path = string.Format(ConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
                builder.Configuration.AddJsonFile(AppData.GetPath(path), true, true);
            }

            var defaultRuleConfigFile = AppData.GetPath($"ReverseProxyConfig.json");
            if (!File.Exists(defaultRuleConfigFile))
            {
                using var fileStream = File.Create(defaultRuleConfigFile);
                using var stringWriter = new StreamWriter(fileStream);
                stringWriter.Write(@"{
  ""ReverseProxy"": {
    ""Routes"": {},
    ""Clusters"": {}
  }
}");
            }

            builder.Configuration.AddJsonFile(AppData.GetPath($"ReverseProxyConfig.json"), optional: true, reloadOnChange: true);

            if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
            {
                builder.Configuration.AddJsonFile(AppData.GetPath($"ReverseProxyConfig.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
            }
        };

        ConfigureWebHostBuilder += (IWebHostBuilder webHostBuilder, AppBuildContext buildContext) =>
        {
            webHostBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 500 * 1024 * 1024;//500M
                serverOptions.Limits.MaxRequestBufferSize = 5 * 1024 * 1024;//5M

                serverOptions.ListenAnyIP(8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                if (!buildContext.HostEnvironment.IsDevelopment())
                {
                    serverOptions.ListenAnyIP(8443, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        listenOptions.UseHttps(httpsOptions =>
                        {
                            httpsOptions.UseYaginxLettuceEncrypt(serverOptions.ApplicationServices);
                        });
                    });
                }
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {

                });
                //serverOptions.ConfigureHttpsDefaults(httpConnectionAdapterOptions =>
                //{
                //	//httpConnectionAdapterOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.RequireCertificate;
                //	httpConnectionAdapterOptions.UseYaginxLettuceEncrypt(serverOptions.ApplicationServices);
                //});
            });
        };
    }
}

using AgileLabs;
using Hangfire;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
public partial class YaginxAppConfigure
{
    public static class ReverseProxyBuilder
    {
        public static void ProxyBuilder(IReverseProxyApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var serviceProvider = context.RequestServices;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Yaginx.ProxyForwarder");
                var moinitorInterfacePattern = serviceProvider.GetService<IOptions<MonitorInterafceOption>>()?.Value;
                try
                {
                    var requestPath = context.Request.Path.Value;
                    var isRecordRequestLog = moinitorInterfacePattern.MatchPattern.IsNullOrEmpty() && Regex.IsMatch(requestPath, moinitorInterfacePattern?.MatchPattern, RegexOptions.IgnoreCase);

                    if (!isRecordRequestLog)
                    {
                        await next();
                    }
                    else
                    {
                        var requestLogger = loggerFactory.CreateLogger("Yaginx.ProxyForwarder.RequstLog");
                        var cancellationToken = context.RequestAborted;
                        using (var requestStream = new MemoryStream())
                        using (var responseStream = new MemoryStream())
                        {
                            //处理Request Stream
                            var originalRequestBody = context.Request.Body;
                            context.Request.Body = requestStream;

                            //处理Response Stream
                            var originalResponseBody = context.Response.Body;
                            context.Response.Body = responseStream;

                            //设置RequestBody的新值
                            await originalRequestBody.CopyToAsync(context.Request.Body, 4096, cancellationToken);
                            context.Request.Body.SafeSeekToBegin();

                            await RecordRequestContentAsync(context, requestLogger);

                            await next();

                            await RecordResponseContentAsync(context, requestLogger);

                            context.Response.Body = originalResponseBody;
                            responseStream.SafeSeekToBegin();
                            await responseStream.CopyToAsync(context.Response.Body, 4096, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常信息
                    logger.LogError(0, ex, "An error occurred while processing the proxy request for {Url}", context.Request.GetDisplayUrl());

                    // 向客户端返回错误响应
                    context.Response.StatusCode = 200;
                    var message = $"Yaginx Error, {ex.FullMessage()}";
                    var acceptHeader = context.Request.Headers.Accept.ToString() ?? string.Empty;
                    if (acceptHeader.Contains("application/json"))
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { flag = false, err = message }));
                    }
                    else if (acceptHeader.Contains("text/html"))
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync($"<html><body><h1>{message}</h1></body></html>");
                    }
                    else
                    {
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(message);
                    }
                }
            });
        }
        private static async Task RecordResponseContentAsync(HttpContext context, ILogger requestLogger)
        {
            context.Response.Body.SafeSeekToBegin();
            var responseStreamReader = new StreamReader(context.Response.Body);
            var responseString = await responseStreamReader.ReadToEndAsync();
            requestLogger.LogInformation($"{context.Connection.Id}:RSP:{context.Request.GetDisplayUrl()}:{responseString.GetLimitString(out var _)}");
            context.Response.Body.SafeSeekToBegin();
        }

        private static async Task RecordRequestContentAsync(HttpContext context, ILogger requestLogger)
        {
            var requestStreamReader = new StreamReader(context.Request.Body);
            var requestString = await requestStreamReader.ReadToEndAsync();
            requestLogger.LogInformation($"{context.Connection.Id}:REQ:{context.Request.GetDisplayUrl()}:{requestString.GetLimitString(out var _)}");
            context.Request.Body.SafeSeekToBegin();
        }
    }
}


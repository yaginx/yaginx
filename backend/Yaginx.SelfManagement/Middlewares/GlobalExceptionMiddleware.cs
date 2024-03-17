using AgileLabs;
using Microsoft.AspNetCore.Http.Extensions;

namespace Yaginx.SelfManagement.Middlewares;

internal sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ManagedApiAuthorizationException)
        {
            if (!context.Response.HasStarted)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    flag = false,
                    errorCode = "403",
                    errorMessage = $"地址{context.Request.GetDisplayUrl()}无对应授权",
                    requestId = context.Request.Headers.RequestId
                });
            }
            else
            {
                // 如果Response已经开始，直接记录日志
                _logger.LogWarning($"Response流已经开始, 无法正常反馈异常到请求流");
            }
        }
        catch (Exception ex)
        {
            if (!context.Response.HasStarted)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    flag = false,
                    errorCode = "500",
                    errorMessage = $"地址{context.Request.GetDisplayUrl()}调用异常,联系技术支持分析处理, ex: {ex.FullMessage()}",
                    requestId = context.Request.Headers.RequestId
                });
            }
            else
            {
                // 如果Response已经开始，直接记录日志
                _logger.LogWarning($"Response流已经开始, 无法正常反馈异常到请求流");
            }
        }
    }
}


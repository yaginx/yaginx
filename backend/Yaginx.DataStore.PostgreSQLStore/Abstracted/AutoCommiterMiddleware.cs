using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Yaginx.DataStore.PostgreSQLStore.Abstracted
{
    internal class AutoCommiterMiddleware : IMiddleware
    {
        private readonly ILogger<AutoCommiterMiddleware> _logger;

        public AutoCommiterMiddleware(ILogger<AutoCommiterMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);
            var dbContextCommiter = context.RequestServices.GetService<IDbContextCommiter>();
            if (dbContextCommiter != null && dbContextCommiter.IsDbContextCreated)
            {
                await dbContextCommiter.CommitAsync();
            }
        }
    }
}

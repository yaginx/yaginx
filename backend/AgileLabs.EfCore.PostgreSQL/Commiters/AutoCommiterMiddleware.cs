using Microsoft.AspNetCore.Http;

namespace AgileLabs.EfCore.PostgreSQL.Commiters
{
    public class AutoCommiterMiddleware : IMiddleware
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

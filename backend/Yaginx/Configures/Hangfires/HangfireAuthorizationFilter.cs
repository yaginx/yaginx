using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Yaginx.Infrastructure.Configuration;

namespace Yaginx.Configures.Hangfires;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context)
	{
		var httpContext = context.GetHttpContext();
		var appSettings = httpContext.RequestServices.GetRequiredService<AppSettings>();
		var authenticationConfig = appSettings.Get<AuthenticationConfig>();
		if (!authenticationConfig.UseSecurity)
			return true;

		if (!(httpContext.User?.Identity?.IsAuthenticated ?? false))
		{
			var authorizeService = httpContext.RequestServices.GetRequiredService<IAuthenticationService>();

			var authenticateResult = authorizeService.AuthenticateAsync(httpContext, null).Result;
			return authenticateResult.Succeeded;
		}
		return false;
	}
}
using AgileLabs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Yaginx.Infrastructure.Securities;
using Yaginx.Models.LoginSessionModels;
using Yaginx.Services.Securities;

namespace Yaginx.ApiControllers;

[ApiController, Route("api/account")]
public class AccountController : YaginxControllerBase
{
	private readonly IAuthenticateService _userService;
	private readonly ILogger<AccountController> _logger;
	private readonly TokenSettings _tokenSettings;

	public AccountController(IAuthenticateService userService, ILogger<AccountController> logger, IOptions<TokenSettings> tokenSettings)
	{
		_userService = userService;
		_logger = logger;
		_tokenSettings = tokenSettings.Value;
	}

	/// <summary>
	/// 登录接口
	/// </summary>
	/// <param name="request"></param>
	/// <param name="workContext"></param>
	/// <param name="loginSessionCache"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	[HttpPost, Route("login"), AllowAnonymous]

	public async Task<LoginResult> Login([FromBody] LoginRequest request, [FromServices] IWorkContextCore workContext)
	{
		using var activity = _activitySource.StartActivity("Login");
		var isVerifySuccess = await _userService.Authenticate(request.Name, request.Password);
		if (!isVerifySuccess)
		{
			var msg = $"User {request.Name} login failure";
			_logger.LogWarning(msg);
			// todo: UserLoginErrorEvent
			throw new Exception(msg);

		}
		_logger.LogWarning($"User {request.Name} login success");

		TimeSpan expiration = await GetLoginSessionExpiration(request.Name);
		return await RefreshSessionCacheAndResponseLoginInfo(request.Name, expiration);
	}

	[HttpGet, Route("log_test"), AllowAnonymous]
	public object LogTest([FromServices] ActivitySource activitySource, [FromServices] ILoggerProvider loggerProviders)
	{
		using var activity = activitySource.StartActivity("LogTest");
		_logger.LogInformation("Information Log");
		_logger.LogDebug("Debug Log");
		_logger.LogTrace("Trace Log");
		_logger.LogWarning("Warning Log");
		_logger.LogError("Error Log");
		_logger.LogCritical("Critical Log");
		activity.SetTag("Log Kind Count", 5);

		var activityEvent = new ActivityEvent("Data RetrivedFromCache", tags: new ActivityTagsCollection { new("count", 5) });
		activity.AddEvent(activityEvent);

		return new
		{
			Tracer.CurrentSpan.Context.TraceId,
			Tracer.CurrentSpan.Context.SpanId,
			loggerProviderNames = loggerProviders.GetType().FullName
		};
	}

	/// <summary>
	/// Token续期
	/// </summary>
	/// <param name="workContext"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	[HttpGet, Route("refresh_token"), Authorize]
	public async Task<LoginResult> RefreshToken([FromServices] IWorkContextCore workContext)
	{
		if (!workContext.Identity.IsAuthenticated)
		{
			throw new Exception("Token已过期");
		}

		var expiration = await GetLoginSessionExpiration(workContext.Identity.Id);

		return await RefreshSessionCacheAndResponseLoginInfo(workContext.Identity.Id, expiration);
	}

	#region Utils
	private async Task<TimeSpan> GetLoginSessionExpiration(string accountId)
	{
		//var accountLoginSessionConfig = await _accountConfigRep.GetLoginSessionConfig(accountId);
		//if (accountLoginSessionConfig != null)
		//{
		//	return TimeSpan.FromMinutes(accountLoginSessionConfig.ExpireMinutes);
		//}

		//return _tokenSettings.Expiration;
		return await Task.FromResult(TimeSpan.FromMinutes(10));
	}

	private async Task<LoginResult> RefreshSessionCacheAndResponseLoginInfo(string accountName, TimeSpan expiration)
	{
		(long expiresIn, string token) = await JwtHelper.GenerateToken(accountName, DateTime.Now, expiration, _tokenSettings);

		// 输出Ssid Cookie
		var ssidCookie = new CookieOptions
		{
			Path = "/",
			HttpOnly = true,
			Secure = true,
			Expires = DateTime.Now.AddSeconds(expiration.TotalSeconds)
		};
		Response.Cookies.Append("Token", token, ssidCookie);
		return new LoginResult { UserId = accountName, Name = accountName, Token = token, ExpiresIn = expiresIn };
	}
	#endregion

	[HttpGet, Route("user_info")]
	public async ValueTask<object> GetUserSessionInfo([FromServices] IWorkContextCore workContext)
	{
		await Task.CompletedTask;
		return new
		{
			WorkContextIdentity = workContext.Identity,
			AspNetIdentity = HttpContext.User.Identity
		};
	}
}

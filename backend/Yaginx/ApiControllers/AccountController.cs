using AgileLabs;
using AgileLabs.AspNet.WebApis.Exceptions;
using AgileLabs.Securities;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Security.Claims;
using Yaginx.DomainModels;
using Yaginx.Infrastructure.Securities;
using Yaginx.Models.LoginSessionModels;
using Yaginx.Services.Securities;
using Yaginx.WorkContexts;

namespace Yaginx.ApiControllers;

[ApiController, Route("yaginx/api/account")]
public class AccountController : YaginxControllerBase
{
    private readonly IAuthenticateService _userService;
    private readonly ILogger<AccountController> _logger;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepository _userRepository;
    private readonly TokenSettings _tokenSettings;

    public AccountController(
        IAuthenticateService userService,
        ILogger<AccountController> logger,
        IOptions<TokenSettings> tokenSettings,
        IEncryptionService encryptionService,
        IUserRepository userRepository)
    {
        _userService = userService;
        _logger = logger;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
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
        var userInfo = await _userRepository.GetByEmailAsync(request.Email);
        if (userInfo == null)
        {
            throw new Exception("账户不存在");
        }

        if (userInfo.PasswordHash.IsNullOrEmpty())
        {
            // 如果未设置密码, 直接使用当前的密码
            userInfo.PasswordSalt = _encryptionService.CreateSaltKey(16);
            userInfo.PasswordHash = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");
            await _userRepository.UpdateAsync(userInfo);
        }

        string pwd = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");

        if (userInfo.PasswordHash != pwd)
        {
            throw new ApiException("Wrong email or password");
        }

        //var isVerifySuccess = await _userService.Authenticate(request.Email, request.Password);
        //if (!isVerifySuccess)
        //{
        //	var msg = $"User {request.Email} login failure";
        //	_logger.LogWarning(msg);
        //	// todo: UserLoginErrorEvent
        //	throw new Exception(msg);
        //}

        _logger.LogWarning($"User {request.Email} login success");

        TimeSpan expiration = await GetLoginSessionExpiration(request.Email);
        return await RefreshSessionCacheAndResponseLoginInfo(userInfo.Id.ToString(), userInfo.Email, request.Email, string.Empty, expiration);
    }

    /// <summary>
    /// Token续期
    /// </summary>
    /// <param name="workContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpGet, Route("refresh_token"), Authorize]
    public async Task<LoginResult> RefreshToken([FromServices] IWorkContext workContext)
    {
        if (!workContext.Identity.IsAuthenticated)
        {
            throw new Exception("Token已过期");
        }

        var expiration = await GetLoginSessionExpiration(workContext.Identity.Id);

        return await RefreshSessionCacheAndResponseLoginInfo(workContext.Identity.Id, workContext.Identity.Name, workContext.Identity.Name, string.Empty, expiration);
    }

    [HttpPost, Route("register"), Authorize]
    public async Task Register([FromBody] UserCreateRequest request)
    {
        var userInfo = await _userRepository.GetByEmailAsync(request.Email);
        if (userInfo != null)
        {
            throw new Exception("账户已存在");
        }

        userInfo = new User
        {
            Id = IdGenerator.NextId(),
            Email = request.Email,
            Password = request.Password,
            PasswordSalt = _encryptionService.CreateSaltKey(16)
        };
        userInfo.PasswordHash = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");
        await _userRepository.AddAsync(userInfo);
        await Task.CompletedTask;
    }

    //[HttpPost, Route("init")]
    //public async Task Init()
    //{
    //    var userCount = _userRepository.Count();
    //    if (userCount > 0)
    //    {
    //        throw new Exception("账户已存在");
    //    }

    //    var userInfo = new User
    //    {
    //        Id = IdGenerator.NextId(),
    //        Email = "admin@yaginx.com",
    //        Password = "admin",
    //        PasswordSalt = _encryptionService.CreateSaltKey(16)
    //    };
    //    userInfo.PasswordHash = _encryptionService.CreatePasswordHash(userInfo.Password, userInfo.PasswordSalt, "SHA256");
    //    _userRepository.Add(userInfo);
    //    await Task.CompletedTask;
    //}

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

    private async Task<LoginResult> RefreshSessionCacheAndResponseLoginInfo(string uniqueId, string uniqueName, string email, string ssid, TimeSpan expiration)
    {
        (long expiresIn, string token) = await JwtHelper.GenerateToken(DateTime.Now, expiration, _tokenSettings, claims =>
        {
            if (uniqueId.IsNotNullOrWhitespace())
                claims.TryAdd(new Claim(ClaimNames.UniqueId, uniqueId));
            if (uniqueName.IsNotNullOrWhitespace())
                claims.TryAdd(new Claim(ClaimNames.UniqueName, email));
            if (email.IsNotNullOrWhitespace())
                claims.TryAdd(new Claim(ClaimNames.Email, email));
            if (ssid.IsNotNullOrWhitespace())
                claims.TryAdd(new Claim(ClaimNames.Ssid, ssid));
        });

        // 输出Ssid Cookie
        var ssidCookie = new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddSeconds(expiration.TotalSeconds)
        };
        Response.Cookies.Append(GlobalConsts.TokenCookieKey, token, ssidCookie);
        return new LoginResult { UserId = uniqueName, Name = uniqueName, Token = token, ExpiresIn = expiresIn };
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

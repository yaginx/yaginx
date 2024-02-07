using AgileLabs;
using AgileLabs.AspNet.WebApis;
using AgileLabs.Json;
using AgileLabs.Securities;
using AgileLabs.Sessions;
using AgileLabs.StatusCodes;
using AgileLabs.WebApp.Hosting;
using AgileLabs.WorkContexts.WorkContextProperties;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Yaginx.Infrastructure.Securities
{
    public static class GlobalConsts
    {
        public const string AdminJwtAuthScheme = "AdminJwtAuth";
        public const string AdminCookieAuthScheme = "AdminCookieAuth";

        public const string TokenCookieKey = "yaginx-token";
        public const string TokenHeaderKey = "x-yaginx-token";
    }
    public class ClaimNames
    {
        public const string UniqueId = "uid";
        public const string UniqueName = "uname";
        public const string DisplayName = "dname";
        public const string Token = "token";
        public const string TokenExpireTs = "token_expire_ts";
        public const string Email = "email";
        public const string Ssid = "ssid";//Server SessionId
        public const string AccountType = "atype";
        public const string TenantId = "tntid";
        public const string Ttid = "ttid";
        public const string TenantAccountSystemRole = "tntrole";
    }
    public static class SecurityServiceRegisterExtensions
    {
        public static void ConfigureSecurityServices(this IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddNiusysSecurity(options =>
            {
                options.EncryptionKey = "2218EF6E-7D95-442F-B967-3979B00E9226";
            });

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = GlobalConsts.AdminJwtAuthScheme;
                options.DefaultChallengeScheme = GlobalConsts.AdminJwtAuthScheme;
            });

            var tokenSettings = new TokenSettings();
            buildContext.Configuration.GetSection(nameof(TokenSettings)).Bind(tokenSettings);
            authenticationBuilder.AddJwtBearer(GlobalConsts.AdminJwtAuthScheme, options =>
            {
                if (options.Events == null)
                    options.Events = new JwtBearerEvents();
                options.Events.OnMessageReceived += OnMessageReceived;
                options.Events.OnTokenValidated = OnTokenValidated;
                options.Events.OnChallenge = OnChallenge;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = tokenSettings.GetSecurityKey(),

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = tokenSettings.Issuer,

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = tokenSettings.Audience,

                    // Validate the token expiry
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero
                };
                //options.UseSecurityTokenValidators = true;
                //options.TokenHandlers.Clear();
                //options.TokenHandlers.Add(new WoScmJwtSecurityValidater(JwtBearerDefaults.AuthenticationScheme));
                //options.SecurityTokenValidators.Clear();
                //options.SecurityTokenValidators.Add();
            });
            services.AddAuthorization();
        }

        private static async Task OnMessageReceived(MessageReceivedContext context)
        {
            // 从QueryString读取
            if (!context.Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                context.Token = context.Request.Query["access_token"];
            }

            // 从Cookie读取
            if (context.Token.IsNullOrEmpty())
            {
                context.Token = context.Request.GetCookieValueAs<string>(GlobalConsts.TokenCookieKey);
            }

            // 从Header读取
            if (context.Token.IsNullOrEmpty())
            {
                context.Token = context.Request.GetHeaderValueAs<string>(GlobalConsts.TokenHeaderKey);
            }
            await Task.CompletedTask;
        }

        private static async Task OnChallenge(JwtBearerChallengeContext context)
        {
            //到这里代表认证错误
            var serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Jwt Authentication OnChallenge");

            // Override the response status code.
            // Emit the WWW-Authenticate header.
            context.Response.OnStarting(async state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);
                context.Response.StatusCode = 200;
                context.Response.ContentType = ContentTypes.JsonWithUtf8Content;
                await Task.CompletedTask;
            }, context.HttpContext);

            var requestSession = serviceProvider.GetRequiredService<IRequestSession>();
            if (context.AuthenticateFailure != null)
            {
                var message = new EnvelopMessage<object>(new
                {
                    //FailureMessage = context.AuthenticateFailure.Message,
                    context.Error,
                    context.ErrorDescription,
                    context.ErrorUri
                })
                {
                    Tid = requestSession.Tid
                };

                switch (context.AuthenticateFailure.Message)
                {
                    case "token_not_exists":
                        message.Code = (int)ApiStatusCode.TokenNotExists;
                        message.ErrMsg = "Token不存在";
                        break;
                    case "token_banned":
                        message.Code = (int)ApiStatusCode.TokenBanned;
                        message.ErrMsg = "Token已被另一个Token踢出去";
                        break;
                    case "authorization_fail":
                        message.Code = (int)ApiStatusCode.AccessForbidden;
                        message.ErrMsg = "无接口访问权限";
                        break;
                    case "auth_ignore_for_current_interface":
                        message.Code = (int)ApiStatusCode.AuthNotRequired;
                        message.ErrMsg = "无接口访问权限";
                        break;
                    default:
                        if (context.AuthenticateFailure.Message.StartsWith($"IDX10223"))
                        {
                            message.Code = (int)ApiStatusCode.TokenExpired;
                            message.ErrMsg = "Token过期, 请重新登录";
                        }
                        else if (context.AuthenticateFailure.Message.StartsWith($"IDX"))
                        {
                            message.Code = (int)ApiStatusCode.TokenIdxIssue;
                            message.ErrMsg = $"不明确的Token问题, 代码{context.AuthenticateFailure.Message.Substring(0, 8)}";
                        }
                        else if (!context.AuthenticateFailure.Message.IsNullOrWhitespace())
                        {
                            message.Code = (int)ApiStatusCode.TokenIssue;
                            message.ErrMsg = $"不明确的Token问题, 错误信息:{context.AuthenticateFailure.Message}";
                        }
                        break;
                }

                //如果没有指定友好提示信息,使用ApiStatusCode默认的提示信息
                if (message.Msg.IsNullOrWhitespace())
                {
                    message.Msg = ((ApiStatusCode)message.Code).GetSuggestionHint();
                }

                //日志打印验证失败的异常信息
                if (context.AuthenticateFailure != null)
                {
                    logger.LogInformation(context.AuthenticateFailure, context.AuthenticateFailure.FullMessage());
                }

                await ResponseMessage(context, message);
            }
            else
            {
                var message = new EnvelopMessage<object>()
                {
                    Tid = requestSession.Tid,
                    Code = (int)ApiStatusCode.TokenNotExists,
                    ErrMsg = $"Token不存在",
                    Data = new
                    {
                        context.Error,
                        context.ErrorDescription,
                        context.ErrorUri
                    }
                };

                await ResponseMessage(context, message);
            }
            context.HandleResponse();
        }

        private static async Task ResponseMessage(JwtBearerChallengeContext context, EnvelopMessage<object> message)
        {
            string outputContentType = context.Request.Headers.Accept;
            if (outputContentType.IndexOf('*') != -1)
            {
                outputContentType = string.Empty;
            }

            if (outputContentType.IsNullOrEmpty())
            {
                string requestContentType = context.Request.Headers.ContentType;
                if (requestContentType.IsNotNullOrWhitespace())
                {
                    outputContentType = requestContentType.Split(';')[0];
                }
            }

            switch (outputContentType.ToLower())
            {
                case ContentTypes.JsonContentType:
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(message, JsonNetSerializerSettings.Instance));
                    break;
                default:
                    await context.Response.WriteAsync($"auth failure: {context.Request.GetDisplayUrl()}");
                    break;
            }
        }

        private static Task OnTokenValidated(TokenValidatedContext context)
        {
            context.Success();
            var identity = context.Principal?.Identity as ClaimsIdentity ?? throw new Exception($"Identity类型错误, 实际Identity类型: {context.Principal?.Identity.GetType().FullName ?? "Identity为null"}");

            // 同步到WorkContext
            identity.SyncToWorkContextIdentityInfo(context.HttpContext.RequestServices);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据Identity设置WorkContext的Identity信息
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="identity"></param>
        public static void SyncToWorkContextIdentityInfo(this ClaimsIdentity identity, IServiceProvider serviceProvider)
        {
            var workContextSetter = serviceProvider.GetRequiredService<IWorkContextCoreSetter>();
            if (workContextSetter != null)
                workContextSetter.SetIdentity(new WorkContextIdentityInfo()
                {
                    IsAuthenticated = identity.IsAuthenticated,
                    Id = identity.GetClaimValueAs<string>(ClaimNames.UniqueId),
                    Name = identity.GetClaimValueAs<string>(ClaimNames.UniqueName),
                    Ssid = identity.GetClaimValueAs<string>(ClaimNames.Ssid),
                });
        }
    }
}
using Yaginx.Infrastructure.Configuration;

namespace Yaginx.Services.Securities
{
	public class AuthenticateService : IAuthenticateService
	{
		private readonly AppSettings _appSettings;

		public AuthenticateService(AppSettings appSettings)
		{
			_appSettings = appSettings;
		}
		public async Task<bool> Authenticate(string username, string password)
		{
			await Task.CompletedTask;
			var _authenticationConfig = _appSettings.Get<AuthenticationConfig>();
			return _authenticationConfig.UserName.Equals(username) && _authenticationConfig.Password.Equals(password);
		}
	}
}

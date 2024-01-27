namespace Yaginx.Services.Securities
{
	public interface IAuthenticateService
	{
		Task<bool> Authenticate(string username, string password);
	}
}

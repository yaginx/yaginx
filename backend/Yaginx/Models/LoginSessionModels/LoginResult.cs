namespace Yaginx.Models.LoginSessionModels
{
	public class LoginResult
	{
		public string UserId { get; set; }
		public string Name { get; set; }
		public long ExpiresIn { get; set; }
		public string Token { get; set; }
	}
}

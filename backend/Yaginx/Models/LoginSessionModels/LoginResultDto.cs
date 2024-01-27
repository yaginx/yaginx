namespace Yaginx.Models.LoginSessionModels
{
	public class LoginResultDto
	{
		public long UserId { get; set; }
		public string DisplayName { get; set; }
		public long ExpiresIn { get; set; }
		public string Token { get; set; }
	}
}

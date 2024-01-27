// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yaginx.Models.LoginSessionModels
{
	public class UserSessionInfoResult
	{
		public long AccountId { get; set; }
		public string UserName { get; set; }
		public string[] Permissions { get; set; }
	}
}

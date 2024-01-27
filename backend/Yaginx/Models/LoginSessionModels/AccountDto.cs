// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yaginx.Models.LoginSessionModels
{
	public class AccountDto
	{
		public long UserId { get; set; }

		/// <summary>
		/// 全局级别的用户名
		/// </summary>
		public string GlobalUserName { get; set; }

		/// <summary>
		/// 系统显示名称
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// 登录用Email
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 手机号
		/// </summary>
		public string MobilePhone { get; set; }

		/// <summary>
		/// Hash加密之后的密码
		/// </summary>
		public string PasswordHash { get; set; }

		/// <summary>
		/// Gets or sets the password salt
		/// </summary>
		public string PasswordSalt { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime? UpdateTime { get; set; }

		public string LastIp { get; set; }
		public DateTime? LastLoginTime { get; set; }

		public bool IsActive { get; set; }
		public bool IsEmailVerified { get; set; }
		public bool IsMobileVerified { get; set; }

		/// <summary>
		/// 验证类型
		/// </summary>
		public string VerifyType { get; set; }
		/// <summary>
		/// 验证Token(随机)
		/// </summary>
		public string VerifyToken { get; set; }

		/// <summary>
		/// 验证过期时间
		/// </summary>
		public DateTime? VerifyExpireTime { get; set; }

		public string RegisterIp { get; set; }
		public string Memo { get; set; }
		/// <summary>
		/// 注册介绍人
		/// </summary>
		public string Recommender { get; set; }

		/// <summary>
		/// 个人邀请码
		/// </summary>
		public string InviteCode { get; set; }
	}
}

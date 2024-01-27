using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Yaginx.Infrastructure.Securities
{
	public class TokenSettings
	{
		/// <summary>
		///  The Issuer (iss) claim for generated tokens.
		/// </summary>
		public string Issuer { get; set; } = "Niusys Cms Management Apis";

		/// <summary>
		/// The Audience (aud) claim for the generated tokens.
		/// </summary>
		public string Audience { get; set; } = "Niusys \"Cms Management Cliet";

		public int ExpirationMinutes { get; set; } = 60 * 8;

		/// <summary>
		/// The expiration time for the generated tokens.
		/// </summary>
		/// <remarks>The default is five minutes (300 seconds).</remarks>
		public TimeSpan Expiration => TimeSpan.FromMinutes(ExpirationMinutes);

		public string SigningKey { get; set; } = $"{{6A30BC68-083D-4DB5-B729-E14F2CA4D396}}";

		public SymmetricSecurityKey GetSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningKey));
		}

		public SigningCredentials GetSigningCredentials()
		{
			return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
		}

		/// <summary>
		/// 是否验证Session有效性
		/// </summary>
		public bool IsVerifyTokenPerRequest { get; set; }

		/// <summary>
		/// CachedDeviceSession在Cache中缓存的时间
		/// </summary>
		public int SessionMemoryCacheSeconds { get; set; }
	}
}
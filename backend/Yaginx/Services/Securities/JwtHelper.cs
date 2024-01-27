using AgileLabs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Yaginx.Infrastructure.Securities;

namespace Yaginx.Services.Securities
{
	public static class JwtHelper
	{
		public static async Task<(long expiresIn, string token)> GenerateToken(string displayName, DateTime issued, TimeSpan expiration, TokenSettings tokenSettings, Action<List<Claim>> claimHandler = null)
		{
			// Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
			// You can add other claims here, if you want:
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Name,displayName),
				new Claim(JwtRegisteredClaimNames.Iat,issued.GetEpochSeconds().ToString(), ClaimValueTypes.Integer64),
			};

			if (claimHandler != null)
				claimHandler(claims);

			// Create the JWT and write it to a string
			var jwt = new JwtSecurityToken(
				issuer: tokenSettings.Issuer,
				audience: tokenSettings.Audience,
				claims: claims,
				notBefore: issued.AddSeconds(-30),
				expires: issued.Add(expiration).AddSeconds(30),
				signingCredentials: tokenSettings.GetSigningCredentials());

			var token = new JwtSecurityTokenHandler().WriteToken(jwt);

			await Task.CompletedTask;
			return (Convert.ToInt64(expiration.TotalSeconds), token);
		}
	}
}

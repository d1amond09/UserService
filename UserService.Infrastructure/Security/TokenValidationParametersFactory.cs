using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Infrastructure.Common.Configuration;

namespace UserService.Infrastructure.Security;

public class TokenValidationParametersFactory
{
	public static TokenValidationParameters Create(JwtSettings settings, bool validateLifetime)
	{
		return new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = validateLifetime,
			ValidIssuer = settings.ValidIssuer,
			ValidAudience = settings.ValidAudience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),

			ClockSkew = TimeSpan.Zero
		};
	}
}

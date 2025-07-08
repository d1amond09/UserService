using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using UserService.Infrastructure.Common.Configuration;
using UserService.Infrastructure.Security;

namespace UserService.Infrastructure.Services;

public class TokenService : ITokenService
{
	private readonly JwtSettings _jwtSettings;
	private readonly UserManager<User> _userManager;
	private readonly SymmetricSecurityKey _signingKey;

	public TokenService(IOptions<JwtSettings> jwtOptions, UserManager<User> userManager)
	{
		_jwtSettings = jwtOptions.Value;
		_userManager = userManager;
		_signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
	}

	public async Task<TokenDto> CreateTokensAsync(User user)
	{
		var claims = await GetClaimsAsync(user);
		var accessToken = GenerateAccessToken(claims);
		var refreshToken = GenerateRefreshToken();

		return new TokenDto(accessToken, refreshToken);
	}

	public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
	{
		var tokenValidationParameters = TokenValidationParametersFactory.Create(
			_jwtSettings,
			validateLifetime: false
		);

		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				return null; 
			}

			return principal;
		}
		catch (Exception)
		{
			return null; 
		}
	}

	private string GenerateAccessToken(IEnumerable<Claim> claims)
	{
		var token = new JwtSecurityToken(
			issuer: _jwtSettings.ValidIssuer,
			audience: _jwtSettings.ValidAudience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
			signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private async Task<List<Claim>> GetClaimsAsync(User user)
	{
		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.UserName ?? ""),
			new(ClaimTypes.Email, user.Email ?? "")
		};

		var roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		return claims;
	}

	private string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}

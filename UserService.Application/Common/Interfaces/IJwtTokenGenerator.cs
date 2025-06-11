using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Users;
using System.Security.Claims;

namespace UserService.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
	public string GenerateRefreshToken();
	public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
	public SigningCredentials GetSigningCredentials();
	public List<Claim> GetClaims(User user);
}

using System.Security.Claims;
using UserService.Application.Common.DTOs;
using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces;

public interface ITokenService
{
	Task<TokenDto> CreateTokensAsync(User user);
	ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}

using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.RefreshToken;

public class RefreshTokenHandler(UserManager<User> userManager, ITokenService tokenService)
	: IRequestHandler<RefreshTokenCommand, TokenDto>
{
	public async Task<TokenDto> Handle(RefreshTokenCommand request, CancellationToken ct)
	{
		var principal = tokenService.GetPrincipalFromExpiredToken(request.ExpiredTokenDto.AccessToken)
			?? throw new AuthenticationException("Invalid access token.");

		var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier)
			?? throw new AuthenticationException("Invalid token: user ID is missing.");

		var user = await userManager.FindByIdAsync(userIdStr)
			?? throw new NotFoundException("User associated with the token not found.");

		if (user.RefreshToken != request.ExpiredTokenDto.RefreshToken 
			|| user.RefreshTokenExpiryTime <= DateTime.UtcNow)
			throw new AuthenticationException("Invalid refresh token or it has expired.");

		var newTokens = await tokenService.CreateTokensAsync(user);

		user.RefreshToken = newTokens.RefreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
		await userManager.UpdateAsync(user);

		return newTokens;
	}
}

using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using System.Security.Authentication;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.LoginUser;

public class LoginUserHandler(UserManager<User> userManager, ITokenService tokenService) : IRequestHandler<LoginUserCommand, TokenDto>
{
	private readonly UserManager<User> _userManager = userManager;
	private readonly ITokenService _tokenService = tokenService;

	public async Task<TokenDto> Handle(LoginUserCommand request, CancellationToken ct)
	{
		User? user = await _userManager.FindByNameAsync(request.UserToLogin.UserName ?? "");

		bool isValid = await _userManager.CheckPasswordAsync(user, request.UserToLogin.Password ?? "");
		
		if(user == null || !isValid)
			throw new AuthenticationException("Invalid username or password");

		var tokenDto = await _tokenService.CreateTokensAsync(user);

		user.RefreshToken = tokenDto.RefreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
		await _userManager.UpdateAsync(user);

		return tokenDto;
	}
}
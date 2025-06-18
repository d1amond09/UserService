using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using System.Security.Authentication;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.UseCases.Auth.LoginUser;

public class LoginUserHandler(UserManager<User> userManager, ITokenService tokenService) : IRequestHandler<LoginUserCommand, TokenDto>
{
	private readonly UserManager<User> _userManager = userManager;
	private readonly ITokenService _tokenService = tokenService;

	public async Task<TokenDto> Handle(LoginUserCommand request, CancellationToken ct)
	{
		User user = await _userManager.FindByNameAsync(request.UserToLogin.UserName ?? "")
			?? throw new NotFoundException($"User with username '{request.UserToLogin.UserName}' was not found.");

		bool isValid = await _userManager.CheckPasswordAsync(user, request.UserToLogin.Password ?? "");
		
		if(user == null || !isValid)
			throw new AuthenticationException("Invalid username or password");

		if (!await _userManager.IsEmailConfirmedAsync(user))
			throw new AuthenticationException("You must confirm your email before logging in.");

		var tokenDto = await _tokenService.CreateTokensAsync(user);

		user.RefreshToken = tokenDto.RefreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

		await _userManager.UpdateAsync(user);

		return tokenDto;
	}
}
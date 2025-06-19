using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using System.Security.Authentication;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.UseCases.Auth.LoginUser;

public class LoginUserHandler(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService) : IRequestHandler<LoginUserCommand, TokenDto>
{
	public async Task<TokenDto> Handle(LoginUserCommand request, CancellationToken ct)
	{
		User user = await userManager.FindByNameAsync(request.UserToLogin.UserName ?? "")
			?? throw new NotFoundException($"User with username '{request.UserToLogin.UserName}' was not found.");

		var result = await signInManager.CheckPasswordSignInAsync(user, request.UserToLogin.Password ?? "", lockoutOnFailure: false);

		if (!result.Succeeded)
		{
			if (result.IsLockedOut)
			{
				throw new AuthenticationException("This account has been blocked.");
			}
			if (result.IsNotAllowed)
			{
				throw new AuthenticationException("You must confirm your email before logging in.");
			}

			throw new AuthenticationException("Invalid username or password");
		}

		if (!await userManager.IsEmailConfirmedAsync(user))
			throw new AuthenticationException("You must confirm your email before logging in.");

		var tokenDto = await tokenService.CreateTokensAsync(user);

		user.RefreshToken = tokenDto.RefreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

		await userManager.UpdateAsync(user);

		return tokenDto;
	}
}
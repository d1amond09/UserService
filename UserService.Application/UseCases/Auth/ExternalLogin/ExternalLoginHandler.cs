using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Common.Constants;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ExternalLogin;

public class ExternalLoginCommandHandler(
	UserManager<User> userManager,
	ITokenService tokenService,
	IExternalAuthService externalAuthService)
	: IRequestHandler<ExternalLoginCommand, TokenDto>
{
	public async Task<TokenDto> Handle(ExternalLoginCommand request, CancellationToken ct)
	{
		var externalUser = await externalAuthService.ValidateGoogleTokenAsync(request.IdToken);
		var loginInfo = new UserLoginInfo(externalUser.Provider, externalUser.ProviderKey, externalUser.Provider);

		var user = await FindOrCreateUserAsync(loginInfo, externalUser);

		return await tokenService.CreateTokensAsync(user);
	}

	private async Task<User> FindOrCreateUserAsync(UserLoginInfo loginInfo, ExternalUserDto externalUser)
	{
		var user = await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
		if (user != null)
			return user;

		user = await userManager.FindByEmailAsync(externalUser.Email);
		if (user != null)
		{
			await userManager.AddLoginAsync(user, loginInfo);
			return user;
		}

		return await CreateUserFromExternalLoginAsync(externalUser, loginInfo);
	}

	private async Task<User> CreateUserFromExternalLoginAsync(ExternalUserDto externalUser, UserLoginInfo loginInfo)
	{
		var user = new User(externalUser.Email, externalUser.Email)
		{
			FirstName = externalUser.FirstName ?? string.Empty,
			LastName = externalUser.LastName ?? string.Empty,
			EmailConfirmed = true 
		};

		var createResult = await userManager.CreateAsync(user);
		if (!createResult.Succeeded)
		{
			var errors = createResult.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}

		await userManager.AddToRoleAsync(user, Roles.User);
		await userManager.AddLoginAsync(user, loginInfo);

		return user;
	}
}
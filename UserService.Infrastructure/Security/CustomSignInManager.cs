using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Security;

public class CustomSignInManager(
	UserManager<User> userManager,
	IHttpContextAccessor contextAccessor,
	IUserClaimsPrincipalFactory<User> claimsFactory,
	IOptions<IdentityOptions> optionsAccessor,
	ILogger<SignInManager<User>> logger,
	IAuthenticationSchemeProvider schemes,
	IUserConfirmation<User> confirmation)
	: SignInManager<User>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
	protected override async Task<SignInResult?> PreSignInCheck(User user)
	{
		var baseResult = await base.PreSignInCheck(user);
		if (baseResult != null)
		{
			return baseResult;
		}

		if (user.IsBlocked)
		{
			Logger.LogWarning(7, "User {userId} is blocked by an administrator.", user.Id);
			return SignInResult.LockedOut;
		}

		return null;
	}
}

using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;

namespace UserService.Application.UseCases.Auth.FindUserByToken;

public class FindUserByTokenHandler(ITokenValidator tokenValidator, UserManager<User> userManager) :
	IRequestHandler<FindUserByTokenQuery, ApiBaseResponse>
{
	private readonly ITokenValidator _tokenValidator = tokenValidator;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(FindUserByTokenQuery request, CancellationToken cancellationToken)
	{
		var principal = _tokenValidator.GetPrincipalFromExpiredToken(request.TokenDto.AccessToken);
		var user = await _userManager.FindByNameAsync(principal.Identity?.Name!);

		if (user == null ||
			user.RefreshToken != request.TokenDto.RefreshToken ||
			user.RefreshTokenExpiryTime <= DateTime.Now)
			return new ApiBadRequestResponse("");

		return new ApiOkResponse<User>(user);
	}
}
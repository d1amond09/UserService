using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;

namespace UserService.Application.UseCases.Auth.LoginUser;

public class LoginUserHandler(UserManager<User> userManager) : IRequestHandler<LoginUserCommand, ApiBaseResponse>
{
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
	{
		User? user = await _userManager.FindByNameAsync(request.UserToLogin.UserName ?? "");

		if(user == null)
		{
			return new ApiNotFoundResponse("User");
		}

		bool isValid = await _userManager.CheckPasswordAsync(user, request.UserToLogin.Password ?? "");

		(bool, User) result = new(isValid, user);

		return new ApiOkResponse<(bool, User?)>(result);
	}
}
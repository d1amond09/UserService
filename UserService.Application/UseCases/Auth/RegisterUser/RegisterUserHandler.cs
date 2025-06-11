using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using AutoMapper;
using MediatR;
using UserService.Domain.Common.Constants;

namespace UserService.Application.UseCases.Auth.RegisterUser;

public class RegisterUserHandler(UserManager<User> userManager, IMapper mapper) : IRequestHandler<RegisterUserCommand, ApiBaseResponse>
{
	private readonly UserManager<User> _userManager = userManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
	{
		var user = _mapper.Map<User>(request.UserForRegistrationDto);

		string? password = request.UserForRegistrationDto.Password;

		if(password == null)
		{
			return new ApiBadRequestResponse("Password is null");
		}

		var identityResult = await _userManager.CreateAsync(user, password);

		if (identityResult.Succeeded)
		{
			await _userManager.AddToRolesAsync(user, [Roles.User]);
		}

		(IdentityResult, User) result = new(identityResult, user);

		return new ApiOkResponse<(IdentityResult, User)>(result);
	}
}
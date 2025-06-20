using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;

namespace UserService.Application.UseCases.Users.AddUserToRole;

public class AddUserToRoleHandler(
	UserManager<User> userManager, 
	RoleManager<Role> roleManager,
	IUserCacheService userCacheService) 
	: IRequestHandler<AddUserToRoleCommand>
{
	public async Task Handle(AddUserToRoleCommand request, CancellationToken ct)
	{
		User user = await userManager.FindByIdAsync(request.UserId.ToString())
			?? throw new NotFoundException($"User with ID '{request.UserId}'");

		if (!await roleManager.RoleExistsAsync(request.RoleName))
			throw new BadRequestException($"Role '{request.RoleName}' does not exist.");

		if (await userManager.IsInRoleAsync(user, request.RoleName))
			return;

		var result = await userManager.AddToRoleAsync(user, request.RoleName);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}

		await userCacheService.InvalidateUserCacheAsync(request.UserId, ct);
	}
}
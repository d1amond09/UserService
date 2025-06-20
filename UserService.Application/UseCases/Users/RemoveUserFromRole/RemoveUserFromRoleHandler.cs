using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.RemoveUserFromRole;

public class RemoveUserFromRoleHandler(UserManager<User> userManager, RoleManager<Role> roleManager) 
	: IRequestHandler<RemoveUserFromRoleCommand>
{
	public async Task Handle(RemoveUserFromRoleCommand request, CancellationToken ct)
	{
		if (!await roleManager.RoleExistsAsync(request.RoleName))
			throw new BadRequestException($"Role '{request.RoleName}' does not exist.");

		User user = await userManager.FindByIdAsync(request.UserId.ToString())
			?? throw new NotFoundException($"User with ID '{request.UserId}'");

		if (!await userManager.IsInRoleAsync(user, request.RoleName))
			throw new BadRequestException($"User is not in role '{request.RoleName}'.");

		var result = await userManager.RemoveFromRoleAsync(user, request.RoleName);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}
	}
}
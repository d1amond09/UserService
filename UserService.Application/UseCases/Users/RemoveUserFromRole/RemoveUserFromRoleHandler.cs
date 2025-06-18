using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.RemoveUserFromRole;

public class RemoveUserFromRoleHandler(IUserRepository userRep, UserManager<User> userManager, RoleManager<Role> roleManager) 
	: IRequestHandler<RemoveUserFromRoleCommand>
{
	private readonly IUserRepository _userRep = userRep;
	private readonly UserManager<User> _userManager = userManager;
	private readonly RoleManager<Role> _roleManager = roleManager; 

	public async Task Handle(RemoveUserFromRoleCommand request, CancellationToken ct)
	{
		User user = await _userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException($"User with ID '{request.UserId}'");

		if (!await _roleManager.RoleExistsAsync(request.RoleName))
			throw new BadRequestException($"Role '{request.RoleName}' does not exist.");

		if (await _userManager.IsInRoleAsync(user, request.RoleName))
			return;

		var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}
	}
}
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.AddUserToRole;

public class AddUserToRoleHandler(IUserRepository userRep, UserManager<User> userManager, RoleManager<Role> roleManager) 
	: IRequestHandler<AddUserToRoleCommand>
{
	private readonly IUserRepository _userRep = userRep;
	private readonly UserManager<User> _userManager = userManager;
	private readonly RoleManager<Role> _roleManager = roleManager; 

	public async Task Handle(AddUserToRoleCommand request, CancellationToken ct)
	{
		User user = await _userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException($"User with ID '{request.UserId}'");

		if (!await _roleManager.RoleExistsAsync(request.RoleName))
			throw new BadRequestException($"Role '{request.RoleName}' does not exist.");

		if (await _userManager.IsInRoleAsync(user, request.RoleName))
			return;

		var result = await _userManager.AddToRoleAsync(user, request.RoleName);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}
	}
}
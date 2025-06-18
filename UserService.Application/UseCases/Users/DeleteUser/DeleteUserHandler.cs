using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.DeleteUser;

public class DeleteUserHandler(ICurrentUserService currentUserService, IUserRepository userRep, UserManager<User> userManager)
	: IRequestHandler<DeleteUserCommand>
{
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly IUserRepository _userRep = userRep;
	private readonly UserManager<User> _userManager = userManager;

	public async Task Handle(DeleteUserCommand request, CancellationToken ct)
	{
		Guid userId = request.UserId;

		User userToDelete = await _userRep.GetByIdAsync(userId, ct) 
			?? throw new NotFoundException($"User with ID '{userId}' not found.");

		if (userToDelete.Id == _currentUserService.UserId)
			throw new ForbiddenAccessException("Administrators cannot delete themselves.");

		var result = await _userManager.DeleteAsync(userToDelete);

		if (!result.Succeeded)
		{
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException($"Failed to delete user. Errors: {errors}");
		}
	}
}
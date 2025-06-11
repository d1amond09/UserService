using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace UserService.Application.UseCases.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, ApiBaseResponse>
{
	private readonly ICurrentUserService _currentUserService;
	private readonly UserManager<User> _userManager;

	public DeleteUserHandler(ICurrentUserService currentUserService, UserManager<User> userManager)
	{
		_currentUserService = currentUserService;
		_userManager = userManager;
	}

	public async Task<ApiBaseResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can delete users.");
		}

		var userToDelete = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (userToDelete == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		if (userToDelete.Id == _currentUserService.UserId)
		{
			return new ApiBadRequestResponse("Administrators cannot delete themselves.");
		}

		var result = await _userManager.DeleteAsync(userToDelete);

		if (!result.Succeeded)
		{
			var errors = result.Errors.Select(e => e.Description).ToList();
			string messageErrors = string.Join(',', [.. errors]);
			return new ApiBadRequestResponse($"{messageErrors}. Failed to delete user.");
		}

		return new ApiOkResponse<string>("User deleted successfully.");
	}
}
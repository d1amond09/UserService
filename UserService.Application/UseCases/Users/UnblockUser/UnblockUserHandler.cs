using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.UnblockUser;

public class UnblockUserHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, UserManager<User> userManager) 
	: IRequestHandler<UnblockUserCommand, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can unblock users.");
		}

		var userToUnblock = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (userToUnblock == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		userToUnblock.Unblock(); 

		_repManager.Users.Update(userToUnblock);
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("User unblocked successfully.");
	}
}
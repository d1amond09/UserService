using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.BlockUser;

public class BlockUserHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, UserManager<User> userManager) : IRequestHandler<BlockUserCommand, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(BlockUserCommand request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can block users.");
		}

		var userToBlock = await _userManager.FindByIdAsync(request.UserId.ToString());
		
		if (userToBlock == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		if (userToBlock.Id == _currentUserService.UserId)
		{
			return new ApiBadRequestResponse("Administrators cannot block themselves.");
		}

		userToBlock.Block();

		_repManager.Users.Update(userToBlock); 
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("User blocked successfully."); 
	}
}
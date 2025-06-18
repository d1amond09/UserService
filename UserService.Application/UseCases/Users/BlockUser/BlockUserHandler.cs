using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.UseCases.Users.BlockUser;

public class BlockUserHandler(IUserRepository userRep, UserManager<User> userManager) 
	: IRequestHandler<BlockUserCommand>
{
	private readonly IUserRepository _userRep = userRep;
	private readonly UserManager<User> _userManager = userManager;

	public async Task Handle(BlockUserCommand request, CancellationToken ct)
	{
		User userToBlock = await _userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException($"User with ID '{request.UserId}' not found.");

		userToBlock.Block();

		await _userManager.UpdateAsync(userToBlock); 
	}
}
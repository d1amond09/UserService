using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.UseCases.Users.UnblockUser;

public class UnblockUserHandler(IUserRepository userRep, UserManager<User> userManager) 
	: IRequestHandler<UnblockUserCommand>
{
	private readonly IUserRepository _userRep = userRep;
	private readonly UserManager<User> _userManager = userManager;

	public async Task Handle(UnblockUserCommand request, CancellationToken ct)
	{
		User userToUnblock = await _userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException($"User with ID '{request.UserId}' not found.");

		userToUnblock.Unblock();

		await _userManager.UpdateAsync(userToUnblock);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.UpdateUserStatus;

public class UpdateUserStatusCommandHandler(
	UserManager<User> userManager,
	IUserCacheService userCacheService) 
	: IRequestHandler<UpdateUserStatusCommand>
{
	public async Task Handle(UpdateUserStatusCommand request, CancellationToken ct)
	{
		var user = await userManager.FindByIdAsync(request.UserId.ToString())
			?? throw new NotFoundException($"User with ID '{request.UserId}' not found.");

		if (request.Action == UserStatusAction.Block)
			user.Block();
		else 
			user.Unblock();

		var result = await userManager.UpdateAsync(user);

		if (!result.Succeeded)
		{
			var errors = result.Errors
					.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}

		await userCacheService.InvalidateUserCacheAsync(request.UserId, ct);
	}
}

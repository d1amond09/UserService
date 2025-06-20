using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Helpers;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Notifications;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.UpdateUserStatus;

public class UpdateUserStatusCommandHandler(
	UserManager<User> userManager,
	IPublisher publisher) 
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

		var cacheKey = CacheKeyGenerator.GetUserCacheKey(request.UserId);
		await publisher.Publish(new CacheInvalidationNotification(cacheKey), ct);
	}
}

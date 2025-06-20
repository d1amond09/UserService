﻿using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Helpers;
using UserService.Application.Common.Notifications;

namespace UserService.Application.UseCases.Users.AddUserToRole;

public class AddUserToRoleHandler(
	UserManager<User> userManager, 
	RoleManager<Role> roleManager,
	IPublisher publisher) 
	: IRequestHandler<AddUserToRoleCommand>
{
	public async Task Handle(AddUserToRoleCommand request, CancellationToken ct)
	{
		User user = await userManager.FindByIdAsync(request.UserId.ToString())
			?? throw new NotFoundException($"User with ID '{request.UserId}'");

		if (!await roleManager.RoleExistsAsync(request.RoleName))
			throw new BadRequestException($"Role '{request.RoleName}' does not exist.");

		if (await userManager.IsInRoleAsync(user, request.RoleName))
			return;

		var result = await userManager.AddToRoleAsync(user, request.RoleName);

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
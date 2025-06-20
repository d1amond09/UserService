using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Helpers;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Notifications;
using UserService.Application.UseCases.Users.GetUserById;
using UserService.Application.UseCases.Users.UpdateUser;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.UpdateUserByAdmin;

public class UpdateUserByAdminCommandHandler(
	UserManager<User> userManager,
	IMapper mapper,
	IPublisher publisher) : IRequestHandler<UpdateUserByAdminCommand>
{
	public async Task Handle(UpdateUserByAdminCommand request, CancellationToken ct)
	{
		var userToUpdate = await userManager.FindByIdAsync(request.UserId.ToString())
			?? throw new NotFoundException($"User with ID '{request.UserId}' not found.");

		mapper.Map(request.Dto, userToUpdate);

		if (userToUpdate.UserName != request.Dto.UserName)
		{
			var setUserNameResult = await userManager.SetUserNameAsync(userToUpdate, request.Dto.UserName);
			if (!setUserNameResult.Succeeded)
			{
				var errors = setUserNameResult.Errors
					.ToDictionary(e => e.Code, e => new[] { e.Description });
				throw new ValidationException(errors);
			}
		}

		var updateResult = await userManager.UpdateAsync(userToUpdate);

		if (!updateResult.Succeeded)
		{
			var errors = updateResult.Errors
					.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}

		var cacheKey = CacheKeyGenerator.GetUserCacheKey(request.UserId);
		await publisher.Publish(new CacheInvalidationNotification(cacheKey), ct);
	}
}

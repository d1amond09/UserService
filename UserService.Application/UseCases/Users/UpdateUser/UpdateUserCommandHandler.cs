using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.UpdateUser;

public class UpdateUserCommandHandler(
	UserManager<User> userManager,
	ICurrentUserService currentUserService,
	IMapper mapper) : IRequestHandler<UpdateUserCommand>
{
	public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		var userId = currentUserService.UserId
			?? throw new UnauthorizedAccessException();

		var userToUpdate = await userManager.FindByIdAsync(userId.ToString())
			?? throw new NotFoundException($"User with ID '{userId}' not found.");

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
	}
}

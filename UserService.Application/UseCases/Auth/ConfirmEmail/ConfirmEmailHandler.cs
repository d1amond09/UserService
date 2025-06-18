using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ConfirmEmail;

public class ConfirmEmailHandler(UserManager<User> userManager) : IRequestHandler<ConfirmEmailCommand>
{
	public async Task Handle(ConfirmEmailCommand request, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.Token) || request.UserId == Guid.Empty)
			throw new BadRequestException("Invalid confirmation request.");

		var user = await userManager.FindByIdAsync(request.UserId.ToString()) 
			?? throw new BadRequestException("Failed to confirm email.");

		var result = await userManager.ConfirmEmailAsync(user, request.Token);

		if (!result.Succeeded)
		{
			var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}
	}
}

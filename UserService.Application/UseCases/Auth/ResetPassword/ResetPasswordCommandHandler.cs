using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<User> userManager)
	: IRequestHandler<ResetPasswordCommand>
{
	public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
	{
		if (request.Password != request.ConfirmPassword)
			throw new ValidationException("Passwords do not match.");

		var user = await userManager.FindByEmailAsync(request.Email)
			?? throw new ValidationException("Invalid password reset request.");

		var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

		if (!result.Succeeded)
		{
			var errors = string.Join(" ", result.Errors.Select(e => e.Description));
			throw new ValidationException($"Failed to reset password: {errors}");
		}
	}
}

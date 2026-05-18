using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
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

		string decodedToken;
		try
		{
			var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
			decodedToken = Encoding.UTF8.GetString(decodedBytes);
		}
		catch (FormatException)
		{
			throw new ValidationException("Invalid token format.");
		}

		var result = await userManager.ResetPasswordAsync(user, decodedToken, request.Password);

		if (!result.Succeeded)
		{
			var errors = string.Join(" ", result.Errors.Select(e => e.Description));
			throw new ValidationException($"Failed to reset password: {errors}");
		}
	}
}

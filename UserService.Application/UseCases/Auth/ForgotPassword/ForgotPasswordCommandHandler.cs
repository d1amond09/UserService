using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler(
	UserManager<User> userManager,
	IEmailService emailService) : IRequestHandler<ForgotPasswordCommand>
{
	public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByEmailAsync(request.forgotPassword.Email);

		if (user is null || !await userManager.IsEmailConfirmedAsync(user))
			return;

		var token = await userManager.GeneratePasswordResetTokenAsync(user);

		var encodedBytes = Encoding.UTF8.GetBytes(token);
		var validToken = WebEncoders.Base64UrlEncode(encodedBytes);

		await emailService.SendPasswordResetAsync(user, validToken, request.forgotPassword.ClientUri);
	}
}

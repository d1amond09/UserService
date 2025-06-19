using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler(
	UserManager<User> userManager,
	IEmailService emailService) : IRequestHandler<ForgotPasswordCommand>
{
	public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByEmailAsync(request.Email);

		if (user is null || !await userManager.IsEmailConfirmedAsync(user))
			return;

		var token = await userManager.GeneratePasswordResetTokenAsync(user);

		await emailService.SendPasswordResetAsync(user, token);
	}
}

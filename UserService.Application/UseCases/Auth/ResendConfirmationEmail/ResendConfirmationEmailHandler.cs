using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailHandler(
	UserManager<User> userManager,
	IEmailService emailService,
	ILogger<ResendConfirmationEmailHandler> logger)
	: IRequestHandler<ResendConfirmationEmailCommand>
{
	public async Task Handle(ResendConfirmationEmailCommand request, CancellationToken ct)
	{
		logger.LogInformation("Attempting to resend confirmation email to {Email}", request.Email);

		var user = await userManager.FindByEmailAsync(request.Email);

		if (user == null)
		{
			logger.LogWarning("Resend confirmation email requested for a non-existent email: {Email}", request.Email);
			return;
		}

		if (await userManager.IsEmailConfirmedAsync(user))
		{
			logger.LogInformation("Email {Email} is already confirmed. No action taken.", request.Email);
			return;
		}

		var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

		try
		{
			await emailService.SendEmailConfirmationAsync(user, token);
			logger.LogInformation("Confirmation email resent successfully to {Email}", request.Email);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to resend confirmation email to {Email}", request.Email);
		}
	}
}

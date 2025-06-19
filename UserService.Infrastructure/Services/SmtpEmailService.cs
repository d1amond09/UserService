using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using UserService.Infrastructure.Common.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace UserService.Infrastructure.Services;

public class SmtpEmailService(IOptions<EmailSettings> emailOptions, IOptions<WebAppSettings> webAppSettings) : IEmailService
{
	private readonly EmailSettings _emailSettings = emailOptions.Value;
	private readonly WebAppSettings _webAppSettings = webAppSettings.Value;

	public async Task SendEmailConfirmationAsync(User user, string token)
	{
		var confirmationLink = $"{_webAppSettings.FrontendBaseUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

		string subject = "Confirm your account";
		string body = $"<p>Hello {user.UserName},</p>" +
					  $"<p>Please confirm your account by clicking this link: " +
					  $"<a href='{confirmationLink}'>Confirm Email</a></p>" +
					  "<p>Thank you!</p>";

		await SendEmailAsync(user.Email ?? "", subject, body);
	}

	private async Task SendEmailAsync(string to, string subject, string body)
	{
		var email = new MimeMessage();

		email.From.Add(MailboxAddress.Parse(_emailSettings.From));
		email.To.Add(MailboxAddress.Parse(to));
		email.Subject = subject;
		email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(
			_emailSettings.SmtpServer,
			_emailSettings.Port,
			SecureSocketOptions.StartTls
		);
		await smtp.AuthenticateAsync(
			_emailSettings.Username,
			_emailSettings.Password
		);
		await smtp.SendAsync(email);
		await smtp.DisconnectAsync(true);
	}

	public async Task SendPasswordResetAsync(User user, string token)
	{
		var resetLink = $"{_webAppSettings.FrontendBaseUrl}/reset-password?email={Uri.EscapeDataString(user.Email ?? "")}&token={Uri.EscapeDataString(token)}";

		string subject = "Reset your password";
		string body = $"<p>Hello {user.UserName},</p>" +
					  $"<p>To reset your password, please click the link below:</p>" +
					  $"<p><a href='{resetLink}'>Reset Password</a></p>" +
					  "<p>If you did not request a password reset, please ignore this email.</p>";

		await SendEmailAsync(user.Email ?? "", subject, body);
	}
}

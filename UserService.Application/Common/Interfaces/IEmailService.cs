using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces;

public interface IEmailService
{
	Task SendEmailConfirmationAsync	(User user, string token);
}

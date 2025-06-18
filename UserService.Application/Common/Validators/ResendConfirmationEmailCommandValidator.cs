using FluentValidation;
using UserService.Application.UseCases.Auth.ResendConfirmationEmail;

namespace UserService.Application.Common.Validators;

public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
	public ResendConfirmationEmailCommandValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress();
	}
}

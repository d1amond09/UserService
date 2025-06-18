using FluentValidation;
using FluentValidation.Results;
using UserService.Application.UseCases.Auth.LoginUser;
using UserService.Application.UseCases.Auth.RefreshToken;

namespace UserService.Application.Common.Validators;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
	public LoginUserCommandValidator()
	{
		RuleFor(x => x.UserToLogin).NotNull();

		RuleFor(x => x.UserToLogin.UserName)
			.NotEmpty().WithMessage("Username is required.");

		RuleFor(x => x.UserToLogin.Password)
			.NotEmpty().WithMessage("Password is required.");
	}
}

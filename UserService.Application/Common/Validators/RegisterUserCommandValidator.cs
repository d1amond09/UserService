using FluentValidation;
using FluentValidation.Results;
using UserService.Application.UseCases.Auth.RegisterUser;

namespace UserService.Application.Common.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(c => c.UserForRegistrationDto.FirstName)
			.NotEmpty().WithMessage("First name is required.")
			.Length(1, 100).WithMessage("First name must be between 1 and 100 characters.");

		RuleFor(c => c.UserForRegistrationDto.LastName)
			.NotEmpty().WithMessage("Last name is required.")
			.Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.");
	}

	public override ValidationResult Validate(ValidationContext<RegisterUserCommand> context)
	{
		return context.InstanceToValidate.UserForRegistrationDto is null
			? new ValidationResult([new ValidationFailure("UserForRegistrationDto", "UserForRegistrationDto object is null")]) 
			: base.Validate(context);
	}
}

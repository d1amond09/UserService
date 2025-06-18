using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using UserService.Application.UseCases.Auth.RegisterUser;
using UserService.Domain.Users;

namespace UserService.Application.Common.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	private readonly UserManager<User> _userManager;

	public RegisterUserCommandValidator(UserManager<User> userManager)
	{
		_userManager = userManager;

		RuleFor(x => x.UserForRegistrationDto).NotNull();

		RuleFor(x => x.UserForRegistrationDto.UserName)
			.NotEmpty().WithMessage("Username is required.")
			.MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
			.MustAsync(BeUniqueUserName).WithMessage("The specified username is already taken.");

		RuleFor(x => x.UserForRegistrationDto.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("A valid email is required.")
			.MustAsync(BeUniqueEmail).WithMessage("The specified email is already taken.");

		RuleFor(x => x.UserForRegistrationDto.Password)
		   .NotEmpty().WithMessage("Password is required.")
		   .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
		   .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
		   .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
		   .Matches("[0-9]").WithMessage("Password must contain at least one number.")
		   .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");

		RuleFor(x => x.UserForRegistrationDto.ConfirmPassword)
		   .Equal(x => x.UserForRegistrationDto.Password)
		   .WithMessage("The password and confirmation password do not match.");
	}

	public override ValidationResult Validate(ValidationContext<RegisterUserCommand> context)
	{
		return context.InstanceToValidate.UserForRegistrationDto is null
			? new ValidationResult([new ValidationFailure("UserForRegistrationDto", "UserForRegistrationDto object is null")]) 
			: base.Validate(context);
	}

	private async Task<bool> BeUniqueUserName(string username, CancellationToken cancellationToken) =>
		await _userManager.FindByNameAsync(username) == null;

	private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken) =>
		await _userManager.FindByEmailAsync(email) == null;
}

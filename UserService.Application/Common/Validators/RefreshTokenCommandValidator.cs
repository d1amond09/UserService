using FluentValidation;
using FluentValidation.Results;
using UserService.Application.UseCases.Auth.RefreshToken;

namespace UserService.Application.Common.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
	public RefreshTokenCommandValidator()
	{
		RuleFor(x => x.ExpiredTokenDto).NotNull();

		RuleFor(x => x.ExpiredTokenDto.AccessToken)
			.NotEmpty().WithMessage("Access token is required.");

		RuleFor(x => x.ExpiredTokenDto.RefreshToken)
			.NotEmpty().WithMessage("Refresh token is required.");
	}
}

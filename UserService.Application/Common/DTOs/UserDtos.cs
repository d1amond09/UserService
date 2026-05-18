using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Common.DTOs;

public record UserDtos
{
	public Guid Id { get; init; }
	public string? UserName { get; init; }
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public string? Email { get; init; }
}

public record UpdateUserDto(string FirstName, string LastName, string UserName,	Guid? PictureId);

public record UserForRegistrationDto
{
	public string? FirstName { get; init; }
	public string? LastName { get; init; }

	[Required(ErrorMessage = "Username is required")]
	public string? UserName { get; init; }

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Password is required")]
	public string? Password { get; init; }

	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
	public string? ConfirmPassword { get; init; }

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress]
	public string? Email { get; init; }

	[Required(ErrorMessage = "Client URI is required")]
	public string? ClientUri { get; init; }
}

public record UserForLoginDto
{
	[Required(ErrorMessage = "EmailOrUserName is required")]
	public string? EmailOrUserName { get; init; }

	[Required(ErrorMessage = "Password name is required")]
	public string? Password { get; init; }
}

public record UserSummaryDto(Guid Id, string UserName, string Email);

public record UserDetailsDto(Guid Id, string UserName, string Email, string? FirstName, string? LastName)
{
	public bool IsBlocked { get; init; }
	public string? PictureUrl { get; set; }
	public IList<string> Roles { get; set; } = [];
}

public record IsUserBlockedDto(bool IsBLocked);

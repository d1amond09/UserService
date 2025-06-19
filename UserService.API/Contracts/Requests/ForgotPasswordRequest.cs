using System.ComponentModel.DataAnnotations;

namespace UserService.API.Contracts.Requests;

public record ForgotPasswordRequest
{
	[Required]
	[EmailAddress]
	public string Email { get; init; } = string.Empty;
}

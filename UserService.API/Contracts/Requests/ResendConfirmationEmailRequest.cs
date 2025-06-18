using System.ComponentModel.DataAnnotations;

namespace UserService.API.Contracts.Requests;

public record ResendConfirmationEmailRequest
{
    [Required]
	[EmailAddress]
	public string Email { get; init; } = string.Empty;
}

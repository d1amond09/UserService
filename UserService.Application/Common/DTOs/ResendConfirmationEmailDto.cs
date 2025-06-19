using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Common.DTOs;

public record ResendConfirmationEmailDto
{
    [Required]
	[EmailAddress]
	public string Email { get; init; } = string.Empty;
}

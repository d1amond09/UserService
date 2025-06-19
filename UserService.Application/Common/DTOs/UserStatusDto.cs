using System.ComponentModel.DataAnnotations;
using UserService.Application.Common.Enums;

namespace UserService.Application.Common.DTOs;

public record UserStatusDto
{
	[Required]
	public UserStatusAction Action { get; init; }
}

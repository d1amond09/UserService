using System.Security.Claims;
using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces;

public interface ICurrentUserService
{
	Guid? UserId { get; }
	string? UserName { get; }
	string? UserEmail { get; }
	bool IsAuthenticated { get; }
	IEnumerable<Claim>? Claims { get; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Common.Constants;
using UserService.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace UserService.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

	public Guid? UserId
	{
		get
		{
			var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
			return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
		}
	}

	public string? UserName => User?.FindFirstValue(ClaimTypes.Name);

	public string? UserEmail => User?.FindFirstValue(ClaimTypes.Email);

	public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

	public IEnumerable<Claim>? Claims => User?.Claims;
}

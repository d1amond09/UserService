using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Middlewares;

public class UserStatusMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task InvokeAsync(HttpContext context, UserManager<User> userManager)
	{
		if (context.User.Identity?.IsAuthenticated == true)
		{
			var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (Guid.TryParse(userIdString, out var userId))
			{
				var user = await userManager.FindByIdAsync(userId.ToString());

				if (user == null || user.IsBlocked)
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsync("User account is blocked or does not exist.");
					return; 
				}
			}
		}

		await _next(context);
	}
}

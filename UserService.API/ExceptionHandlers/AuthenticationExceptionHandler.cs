using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace UserService.API.ExceptionHandlers;

public class AuthenticationExceptionHandler(ILogger<AuthenticationExceptionHandler> logger) 
	: IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		
		if (exception is not AuthenticationException authException)
		{
			return false; 
		}

		logger.LogInformation(
			"Authentication failed for {Method} {Path}: {Message}",
			httpContext.Request.Method,
			httpContext.Request.Path,
			authException.Message);

		httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

		await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
		{
			Status = StatusCodes.Status401Unauthorized,
			Title = "Authentication Failed",
			Detail = authException.Message, 
			Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
			Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
		}, ct);

		return true;
	}
}

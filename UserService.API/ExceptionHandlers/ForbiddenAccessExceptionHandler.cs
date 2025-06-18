using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Exceptions;

namespace UserService.API.ExceptionHandlers;

public class ForbiddenAccessExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		if (exception is not ForbiddenAccessException accessException)
		{
			return false;
		}

		httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

		await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
		{
			Status = StatusCodes.Status403Forbidden,
			Title = "Forbidden",
			Detail = accessException.Message,
			Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
			Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
		}, ct);

		return true;
	}
}


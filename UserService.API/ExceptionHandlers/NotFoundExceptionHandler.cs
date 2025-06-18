using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Exceptions;

namespace UserService.API.ExceptionHandlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		if (exception is not NotFoundException notFoundException)
		{
			return false;
		}

		httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

		await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
		{
			Status = StatusCodes.Status404NotFound,
			Title = "Not Found",
			Detail = notFoundException.Message,
			Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
			Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
		}, ct);

		return true;
	}
}


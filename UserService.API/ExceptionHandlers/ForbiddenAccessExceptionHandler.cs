using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Exceptions;

namespace UserService.API.ExceptionHandlers;

public class ForbiddenAccessExceptionHandler(ILogger<ForbiddenAccessExceptionHandler> logger) : IExceptionHandler
{
	private readonly ILogger<ForbiddenAccessExceptionHandler> _logger = logger;
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		if (exception is not ForbiddenAccessException accessException)
		{
			return false;
		}

		_logger.LogWarning(
				"Access was forbidden for {Method} {Path}. Errors: {@Errors}",
				httpContext.Request.Method,
				httpContext.Request.Path,
				accessException.Message);

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


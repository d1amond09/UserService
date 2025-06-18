using System.Threading;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Exceptions;

namespace UserService.API.ExceptionHandlers;

public class BadRequestExceptionHandler(ILogger<AuthenticationExceptionHandler> logger) : IExceptionHandler
{
	private readonly ILogger<AuthenticationExceptionHandler> _logger = logger;
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		if (exception is not BadRequestException badRequestException)
		{
			return false;
		}

		_logger.LogWarning(
				"Request failed for {Method} {Path}. Errors: {@Errors}",
				httpContext.Request.Method,
				httpContext.Request.Path,
				badRequestException.Message);

		httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "Bad Request",
			Detail = badRequestException.Message,
			Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
		};

		if (exception is ValidationException validationException)
		{
			problemDetails.Extensions.Add("errors", validationException.Errors);

			_logger.LogWarning(
				"Validation failed for {Method} {Path}. Errors: {@Errors}",
				httpContext.Request.Method,
				httpContext.Request.Path,
				validationException.Errors);
		}
		

		await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

		return true;
	}
}


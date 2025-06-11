using UserService.Application.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using UserService.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
	protected IActionResult ProcessError(ApiBaseResponse baseResponse)
	{
		return baseResponse switch
		{
			ApiForbiddenResponse => BadRequest(new ErrorDetails
			{
				Message = ((ApiForbiddenResponse)baseResponse).Message,
				StatusCode = StatusCodes.Status403Forbidden
			}),
			ApiNotFoundResponse => NotFound(new ErrorDetails
			{
				Message = ((ApiNotFoundResponse)baseResponse).Message,
				StatusCode = StatusCodes.Status404NotFound
			}),
			ApiBadRequestResponse => BadRequest(new ErrorDetails
			{
				Message = ((ApiBadRequestResponse)baseResponse).Message,
				StatusCode = StatusCodes.Status400BadRequest
			}),
			_ => throw new NotImplementedException()
		};
	}
}

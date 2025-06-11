using UserService.Domain.Common.RequestFeatures.ModelParameters;
using UserService.Application.UseCases.Users.SetUserRole;
using UserService.Application.UseCases.Users.UnblockUser;
using UserService.Application.UseCases.Users.DeleteUser;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.UseCases.Users.BlockUser;
using UserService.Application.UseCases.Users.GetUsers;
using UserService.Application.Common.DTOs;
using UserService.Domain.Common.RequestFeatures;
using UserService.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using UserService.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MediatR;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> GetUserMe()
	{
		var result = await _sender.Send(new GetUserMeQuery());
		return result.Success
			? Ok(result.GetResult<UserDetailsDto>())
			: ProcessError(result);
	}

	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] UserParameters userParameters)
	{
		var baseResult = await _sender.Send(new GetUsersQuery(userParameters));
		
		if (!baseResult.Success)
			return ProcessError(baseResult);

		var (userDtos, metaData) = baseResult.GetResult<(List<UserDetailsDto>, MetaData)>();

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

		return Ok(userDtos);
	}

	[HttpPut("{id:guid}/block")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> BlockUser(Guid id)
	{
		var result = await _sender.Send(new BlockUserCommand(id));
		return result.Success ? Ok() : ProcessError(result); 
	}

	[HttpPut("{id:guid}/unblock")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UnblockUser(Guid id)
	{
		var result = await _sender.Send(new UnblockUserCommand(id));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)] 
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)] 
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteUser(Guid id)
	{
		var result = await _sender.Send(new DeleteUserCommand(id));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPost("set-role")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetUserRole([FromBody] SetUserRoleCommand command) 
	{
		var result = await _sender.Send(command);
		return result.Success ? Ok() : ProcessError(result);
	}
}

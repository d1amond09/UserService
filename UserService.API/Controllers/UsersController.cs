using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.UseCases.Users.DeleteUser;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.UseCases.Users.GetUsers;
using UserService.Application.Common.DTOs;
using UserService.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MediatR;
using UserService.Application.UseCases.Users.GetUserById;
using UserService.Application.UseCases.Users.UpdateUser;
using UserService.Application.UseCases.Users.UpdateUserByAdmin;
using UserService.Application.UseCases.Users.UpdateUserStatus;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(ISender sender) : ControllerBase
{
	private readonly ISender _sender = sender;

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> GetUserMe()
	{
		var userDetails = await _sender.Send(new GetUserMeQuery());
		return Ok(userDetails);
	}

	[HttpGet("{id:guid}", Name = "GetUserById")]
	public async Task<IActionResult> GetUserById(Guid id)
	{
		var userDetails = await _sender.Send(new GetUserByIdQuery(id));
		return Ok(userDetails);
	}

	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] UserParameters userParameters)
	{
		var userList = await _sender.Send(new GetUsersQuery(userParameters));

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(userList.MetaData));

		return Ok(userList);
	}

	[HttpPut("{id:guid}")] 
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateUserByAdmin(Guid id, [FromBody] UpdateUserDto request)
	{
		await _sender.Send(new UpdateUserByAdminCommand(id, request));
		return NoContent();
	}

	[Authorize]
	[HttpPut("me")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
	{
		await _sender.Send(new UpdateUserCommand(request));
		return NoContent();
	}

	[HttpPatch("{id:guid}/status")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UserStatusDto request)
	{
		var command = new UpdateUserStatusCommand(id, request.Action);
		await _sender.Send(command);

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> DeleteUser(Guid id)
	{
		await _sender.Send(new DeleteUserCommand(id));
		return NoContent();
	}
}

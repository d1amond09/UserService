using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.UseCases.Users.UnblockUser;
using UserService.Application.UseCases.Users.DeleteUser;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.UseCases.Users.BlockUser;
using UserService.Application.UseCases.Users.GetUsers;
using UserService.Application.Common.DTOs;
using UserService.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MediatR;
using UserService.Application.Common.RequestFeatures;
using UserService.Application.UseCases.Users.AddUserToRole;
using UserService.Application.UseCases.Users.GetUserById;

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

	[HttpPut("{id:guid}/block")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> BlockUser(Guid id)
	{
		await _sender.Send(new BlockUserCommand(id));
		return NoContent();
	}

	[HttpPut("{id:guid}/unblock")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> UnblockUser(Guid id)
	{
		await _sender.Send(new UnblockUserCommand(id));
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

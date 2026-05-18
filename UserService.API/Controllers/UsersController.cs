using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.UseCases.Users.DeleteAvatar;
using UserService.Application.UseCases.Users.DeleteUser;
using UserService.Application.UseCases.Users.GetUserById;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.UseCases.Users.GetUsers;
using UserService.Application.UseCases.Users.IsUserBlocked;
using UserService.Application.UseCases.Users.UpdateUser;
using UserService.Application.UseCases.Users.UpdateUserByAdmin;
using UserService.Application.UseCases.Users.UpdateUserStatus;
using UserService.Application.UseCases.Users.UploadAvatar;
using UserService.Domain.Common.Constants;

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

	[Authorize]
	[HttpGet("{id:guid}", Name = "GetUserById")]
	public async Task<IActionResult> GetUserById(Guid id)
	{
		var userDetails = await _sender.Send(new GetUserByIdQuery(id));
		return Ok(userDetails);
	}

	[AllowAnonymous]
	[HttpGet("{id:guid}/is-blocked")]
	public async Task<IActionResult> IsUserBlocked(Guid id)
	{
		var userIsBlocked = await _sender.Send(new IsUserBlockedQuery(id));
		return Ok(userIsBlocked);
	}

	[Authorize(Roles = "Admin")]
	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] UserParameters userParameters)
	{
		var userList = await _sender.Send(new GetUsersQuery(userParameters));

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(userList.MetaData));

		return Ok(userList.Items);
	}

	[Authorize(Roles = "Admin")]
	[HttpPut("{id:guid}")] 
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

	[Authorize]
	[HttpPost("avatar")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> UploadAvatar(IFormFile file)
	{
		var imageUrl = await _sender.Send(new UploadAvatarCommand(file));

		return Ok(new { Url = imageUrl });
	}

	[Authorize]
	[HttpDelete("avatar")]
	public async Task<IActionResult> DeleteAvatar()
	{
		await _sender.Send(new DeleteAvatarCommand());
		return NoContent();
	}

	[Authorize(Roles = "Admin")]
	[HttpPatch("{id:guid}/status")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UserStatusDto request)
	{
		var command = new UpdateUserStatusCommand(id, request.Action);
		await _sender.Send(command);

		return NoContent();
	}

	[Authorize(Roles = Roles.Admin)]
	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> DeleteUser(Guid id)
	{
		await _sender.Send(new DeleteUserCommand(id));
		return NoContent();
	}
}

using UserService.Application.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserService.Application.UseCases.Auth.RefreshToken;

namespace UserService.API.Controllers;

[Consumes("application/json")]
[Route("api/token")]
[ApiController]
public class TokenController(ISender sender) : ControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
	{
		var newTokenDto = await _sender.Send(new RefreshTokenCommand(tokenDto));
		return Ok(newTokenDto);
	}
}

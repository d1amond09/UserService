using UserService.Application.UseCases.Auth.RegisterUser;
using UserService.Application.UseCases.Auth.LoginUser;
using UserService.Application.Common.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Users;
using MediatR;

namespace UserService.API.Controllers;

[Consumes("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController(ISender sender) : ControllerBase
{
	private readonly ISender _sender = sender;
	
	[HttpPost("register")]
	[ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
	{
		var newUserId = await _sender.Send(new RegisterUserCommand(userForRegistration));
		return CreatedAtRoute("GetUserById", new { id = newUserId }, new { id = newUserId });
	}

	[HttpPost("login", Name = "SignIn")]
	public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLogin)
	{
		TokenDto tokenDto = await _sender.Send(new LoginUserCommand(userForLogin));
		return Ok(tokenDto);
	}
}


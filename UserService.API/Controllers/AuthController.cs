using UserService.Application.UseCases.Auth.RegisterUser;
using UserService.Application.UseCases.Auth.CreateToken;
using UserService.Application.UseCases.Auth.LoginUser;
using UserService.Application.Common.DTOs;
using Microsoft.AspNetCore.Identity;
using UserService.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Users;
using MediatR;

namespace UserService.API.Controllers;

[Consumes("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController(ISender sender, UserManager<User> userManager) : ApiControllerBase
{
	private readonly ISender _sender = sender;
	private readonly UserManager<User> _userManager = userManager;

	[HttpPost("register", Name = "SignUp")]
	public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
	{
		var baseResult = await _sender.Send(new RegisterUserCommand(userForRegistration));

		var (result, user) = baseResult.GetResult<(IdentityResult, User)>();

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.TryAddModelError(error.Code, error.Description);
			}
			return BadRequest(ModelState);
		}

		return StatusCode(201);
	}

	[HttpPost("login", Name = "SignIn")]
	public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLogin)
	{
		var baseResult = await _sender.Send(new LoginUserCommand(userForLogin));

		var (isValid, user) = baseResult.GetResult<(bool, User?)>();

		if (!isValid || user == null)
			return Unauthorized("Invalid username or password.");

		var tokenDtoBaseResult = await _sender.Send(new CreateTokenCommand(user, PopulateExp: true));
		var tokenDto = tokenDtoBaseResult.GetResult<TokenDto>();

		return Ok(tokenDto);
	}
}


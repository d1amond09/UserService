using UserService.Application.UseCases.Auth.RegisterUser;
using UserService.Application.UseCases.Auth.LoginUser;
using UserService.Application.Common.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Users;
using MediatR;
using UserService.Application.UseCases.Auth.ConfirmEmail;
using Microsoft.AspNetCore.Identity.Data;
using UserService.Application.UseCases.Auth.ResendConfirmationEmail;
using UserService.Application.UseCases.Auth.ForgotPassword;
using UserService.Application.UseCases.Auth.ResetPassword;
using UserService.Application.UseCases.Auth.ExternalLogin;
using UserService.Application.Common.Interfaces;
using Google.Apis.Auth;

namespace UserService.API.Controllers;

[Consumes("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController(ISender sender) 
	: ControllerBase
{	
	[HttpPost("register")]
	[ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
	{
		var newUserId = await sender.Send(new RegisterUserCommand(userForRegistration));
		return CreatedAtRoute("GetUserById", new { id = newUserId }, new { id = newUserId });
	}

	[HttpPost("login", Name = "SignIn")]
	[ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
	public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLogin)
	{
		TokenDto tokenDto = await sender.Send(new LoginUserCommand(userForLogin));
		return Ok(tokenDto);
	}

	[HttpGet("confirm-email")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
	{
		await sender.Send(new ConfirmEmailCommand(userId, token));

		return Ok(new { Message = "Your email has been successfully confirmed. You can now log in." });
	}

	[HttpPost("resend-confirmation-email")]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailCommand command)
	{
		await sender.Send(command);

		return Accepted();
	}

	[HttpPost("forgot-password")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
	{
		await sender.Send(command);

		return Ok(new { Message = "If an account with this email exists, a password reset link has been sent." });
	}

	[HttpPost("reset-password")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
	{
		await sender.Send(command);

		return Ok(new { Message = "Your password has been successfully reset." });
	}

	[HttpPost("google")]
	[ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GoogleLogin([FromBody] ExternalLoginCommand command)
	{
		var tokenDto = await sender.Send(command);
		return Ok(tokenDto);
	}
}


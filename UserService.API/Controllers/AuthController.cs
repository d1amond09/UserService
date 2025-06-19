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
	[ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
	public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLogin)
	{
		TokenDto tokenDto = await _sender.Send(new LoginUserCommand(userForLogin));
		return Ok(tokenDto);
	}

	[HttpGet("confirm-email")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
	{
		await _sender.Send(new ConfirmEmailCommand(userId, token));

		return Ok(new { Message = "Your email has been successfully confirmed. You can now log in." });
	}

	[HttpPost("resend-confirmation-email")]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDto request)
	{
		await _sender.Send(new ResendConfirmationEmailCommand(request.Email));

		return Accepted();
	}

	[HttpPost("forgot-password")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
	{
		await _sender.Send(new ForgotPasswordCommand(request.Email));

		return Ok(new { Message = "If an account with this email exists, a password reset link has been sent." });
	}

	[HttpPost("reset-password")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
	{
		await _sender.Send(command);
		return Ok(new { Message = "Your password has been successfully reset." });
	}
}


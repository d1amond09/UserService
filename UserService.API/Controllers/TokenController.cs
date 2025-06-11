using UserService.Application.UseCases.Auth.FindUserByToken;
using UserService.Application.UseCases.Auth.CreateToken;
using UserService.Application.Common.DTOs;
using UserService.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Users;
using MediatR;

namespace UserService.API.Controllers;

[Consumes("application/json")]
[Route("api/token")]
[ApiController]
public class TokenController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
	{
		var baseResultRefresh = await _sender.Send(new FindUserByTokenQuery(tokenDto));
		var user = baseResultRefresh.GetResult<User>();

		var baseResult = await _sender.Send(new CreateTokenCommand(user, PopulateExp: false));
		var tokenDtoToReturn = baseResult.GetResult<TokenDto>();

		return Ok(tokenDtoToReturn);
	}
}

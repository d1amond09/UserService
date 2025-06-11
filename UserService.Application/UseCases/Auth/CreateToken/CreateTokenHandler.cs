using UserService.Application.Common.Responses;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using UserService.Domain.Users;
using System.Security.Claims;
using System.Text;
using MediatR;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.CreateToken;

public class CreateTokenHandler(
	IJwtTokenGenerator tokenGenerator,
	UserManager<User> userManager) : IRequestHandler<CreateTokenCommand, ApiBaseResponse>
{
	private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
	{
		var signingCredentials = _tokenGenerator.GetSigningCredentials();
		var claims = _tokenGenerator.GetClaims(request.User);

		var roles = await _userManager.GetRolesAsync(request.User);
		
		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var tokenOptions = _tokenGenerator.GenerateTokenOptions(signingCredentials, claims);
		var refreshToken = _tokenGenerator.GenerateRefreshToken();

		request.User.RefreshToken = refreshToken;

		if (request.PopulateExp)
			request.User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

		await _userManager.UpdateAsync(request.User);

		var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
		var tokenDto = new TokenDto(accessToken, refreshToken);

		return new ApiOkResponse<TokenDto>(tokenDto);
	}

}

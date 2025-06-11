using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserService.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace UserService.Infrastructure.Security.TokenValidation;

public sealed class JwtBearerTokenValidationConfiguration(ITokenValidator tokenValidator)
	: IConfigureNamedOptions<JwtBearerOptions>
{
	private readonly ITokenValidator _tokenValidator = tokenValidator;

	public void Configure(string? name, JwtBearerOptions options) => Configure(options);

	public void Configure(JwtBearerOptions options)
	{
		options.TokenValidationParameters = _tokenValidator.GetTokenValidationParameters();
	}
}

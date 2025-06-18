using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace UserService.Application.Common.Interfaces;

public interface ITokenValidator
{
	TokenValidationParameters GetTokenValidationParameters();
}

using UserService.Application.Common.DTOs;

namespace UserService.Application.Common.Interfaces;

public interface IExternalAuthService
{
	Task<ExternalUserDto> ValidateGoogleTokenAsync(string idToken);
}

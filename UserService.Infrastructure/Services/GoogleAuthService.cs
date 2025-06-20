using System.Security.Authentication;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces;
using UserService.Infrastructure.Common.Configuration;

namespace UserService.Infrastructure.Services;

public class GoogleAuthService(IOptions<GoogleAuthSettings> googleSettings) : IExternalAuthService
{
	private readonly GoogleAuthSettings _googleSettings = googleSettings.Value;
	public async Task<ExternalUserDto> ValidateGoogleTokenAsync(string idToken)
	{
		string googleClientId = _googleSettings.ClientId 
			?? throw new InvalidOperationException("Google ClientId is not configured on the server.");

		try
		{
			var validationSettings = new GoogleJsonWebSignature.ValidationSettings
			{
				Audience = [googleClientId]
			};

			var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

			return new ExternalUserDto(
				"Google",
				payload.Subject,
				payload.Email,
				payload.GivenName,
				payload.FamilyName
			);
		}
		catch (InvalidJwtException ex)
		{
			throw new AuthenticationException("Invalid Google Token.", ex);
		}
	}
}


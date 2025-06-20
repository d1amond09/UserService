namespace UserService.Infrastructure.Common.Configuration;

public class GoogleAuthSettings
{
	public const string SectionName = "Google";

	public string ClientId { get; init; } = string.Empty;
	public string ClientSecret { get; init; } = string.Empty;
}

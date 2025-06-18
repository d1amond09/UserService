namespace UserService.Infrastructure.Security;

public class JwtSettings
{
	public const string SectionName = "JwtSettings";
	public string Secret { get; init; } = string.Empty;
	public string ValidIssuer { get; init; } = string.Empty;
	public string ValidAudience { get; init; } = string.Empty;
	public int ExpiresInMinutes { get; init; }
}

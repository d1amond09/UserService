namespace UserService.Infrastructure.Common.Configuration;

public class CloudinarySettings
{
	public const string SectionName = "Cloudinary";
	public string Cloud { get; init; } = string.Empty;
	public string ApiKey { get; init; } = string.Empty;
	public string ApiSecret { get; init; } = string.Empty;
	public string Folder { get; init; } = string.Empty;
}

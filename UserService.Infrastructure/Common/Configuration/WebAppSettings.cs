﻿namespace UserService.Infrastructure.Common.Configuration;

public class WebAppSettings
{
	public const string SectionName = "WebAppSettings";
	public string BaseUrl { get; init; } = string.Empty;
	public string FrontendBaseUrl { get; init; } = string.Empty;

}

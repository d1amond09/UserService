﻿namespace UserService.Infrastructure.Common.Configuration;

public class EmailSettings
{
	public const string SectionName = "EmailSettings";
	public string From { get; init; } = string.Empty;
	public string SmtpServer { get; init; } = string.Empty;
	public int Port { get; init; }
	public string Username { get; init; } = string.Empty;
	public string Password { get; init; } = string.Empty;
}

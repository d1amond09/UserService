namespace UserService.Application.Common.DTOs;

public class CloudUploadResult
{
	public bool Success { get; set; }
	public string? FilePath { get; set; }
	public string? ErrorMessage { get; set; }
}

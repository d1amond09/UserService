using Microsoft.AspNetCore.Http;

namespace UserService.Application.Common.Interfaces;

public interface ICloudinaryService
{
	Task<(string SecureUrl, string PublicId)?> UploadImageAsync(IFormFile file);
	Task<bool> DeleteImageAsync(string publicId);
}

using UserService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using UserService.Infrastructure.Common.Configuration;
using Microsoft.Extensions.Options;

namespace UserService.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
	private readonly CloudinarySettings _cloudinarySettings;
	private readonly Cloudinary _cloudinary;

	public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
	{
		_cloudinarySettings = cloudinarySettings.Value; 
		Account account = new (_cloudinarySettings.Cloud, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
		_cloudinary = new Cloudinary(account);
		_cloudinary.Api.Secure = true;
	}

	public async Task<(string SecureUrl, string PublicId)?> UploadImageAsync(IFormFile file)
	{
		if (file == null || file.Length == 0) return null;

		await using var stream = file.OpenReadStream();
		var uploadParams = new ImageUploadParams
		{
			File = new FileDescription(file.FileName, stream),
			Folder = _cloudinarySettings.Folder,
		};

		var uploadResult = await _cloudinary.UploadAsync(uploadParams);

		if (uploadResult.Error != null)
		{
			return null;
		}

		return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
	}

	public async Task<bool> DeleteImageAsync(string publicId)
	{
		if (string.IsNullOrEmpty(publicId)) return false;

		var deletionParams = new DeletionParams(publicId);
		try
		{
			var result = await _cloudinary.DestroyAsync(deletionParams);
			if (result.Result == "ok" || result.Result == "not found")
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}

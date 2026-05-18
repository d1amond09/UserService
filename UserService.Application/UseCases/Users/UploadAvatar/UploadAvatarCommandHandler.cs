using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Helpers;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.Common.Notifications;
using UserService.Domain.Common.Constants;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.UploadAvatar;

public class UploadAvatarCommandHandler(
	UserManager<User> userManager,
	ICurrentUserService currentUserService,
	ICloudinaryService cloudinaryService,
	IPictureRepository pictureRepository,
	IPublisher publisher) : IRequestHandler<UploadAvatarCommand, string>
{
	public async Task<string> Handle(UploadAvatarCommand request, CancellationToken ct)
	{
		if (request.File == null || request.File.Length == 0)
			throw new BadRequestException("File is empty.");

		var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };
		if (!allowedContentTypes.Contains(request.File.ContentType))
			throw new BadRequestException("Only JPG, PNG and WEBP images are allowed.");

		Guid userId = currentUserService.UserId
			?? throw new UnauthorizedAccessException();

		var user = await userManager.FindByIdAsync(userId.ToString())
			?? throw new NotFoundException($"User with ID '{userId}' not found.");

		var uploadResult = await cloudinaryService.UploadImageAsync(request.File)
			?? throw new BadRequestException("Image upload failed.");

		var newPicture = new Picture
		{
			Id = Guid.NewGuid(),
			PublicId = uploadResult.PublicId,
			Url = uploadResult.SecureUrl
		};

		pictureRepository.Create(newPicture);
		await pictureRepository.SaveAsync(ct);

		var oldPictureId = user.PictureId;
		user.Picture = null;
		user.PictureId = newPicture.Id;

		var updateResult = await userManager.UpdateAsync(user);

		if (!updateResult.Succeeded)
		{
			await cloudinaryService.DeleteImageAsync(newPicture.PublicId);
			pictureRepository.Delete(newPicture);
			await pictureRepository.SaveAsync(ct);

			var errors = updateResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
			throw new ValidationException(errors);
		}

		var defaultId = Guid.Parse(Pictures.DefaultId);
		if (oldPictureId.HasValue && oldPictureId.Value != defaultId)
		{
			var oldPicture = await pictureRepository.GetByIdAsync(oldPictureId.Value);
			if (oldPicture != null)
			{
				if (!string.IsNullOrEmpty(oldPicture.PublicId))
				{
					await cloudinaryService.DeleteImageAsync(oldPicture.PublicId);
				}
				pictureRepository.Delete(oldPicture);
				await pictureRepository.SaveAsync(ct);
			}
		}


		var cacheKey = CacheKeyGenerator.GetUserCacheKey(userId);
		await publisher.Publish(new CacheInvalidationNotification(cacheKey), ct);

		return newPicture.Url;
	}
}


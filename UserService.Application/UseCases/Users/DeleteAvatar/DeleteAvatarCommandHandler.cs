using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Domain.Common.Constants;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.DeleteAvatar;

public class DeleteAvatarCommandHandler(
	UserManager<User> userManager,
	ICurrentUserService currentUserService,
	ICloudinaryService cloudinaryService,
	IPictureRepository pictureRepository) : IRequestHandler<DeleteAvatarCommand>
{
	public async Task Handle(DeleteAvatarCommand request, CancellationToken ct)
	{
		Guid userId = currentUserService.UserId
			?? throw new UnauthorizedAccessException();

		var user = await userManager.FindByIdAsync(userId.ToString())
			?? throw new NotFoundException("User not found.");

		var oldPictureId = user.PictureId;

		if (!oldPictureId.HasValue || oldPictureId.Value.ToString() == Pictures.DefaultId)
			return;

		user.PictureId = Guid.Parse(Pictures.DefaultId);
		await userManager.UpdateAsync(user);

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
}


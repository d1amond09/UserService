using UserService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService.Infrastructure.Users.Persistence.Configurations;

public class PictureConfiguration : IEntityTypeConfiguration<Picture>
{
	public void Configure(EntityTypeBuilder<Picture> builder)
	{
		builder.HasKey(u => u.Id);
		builder.HasIndex(u => u.Id).IsUnique();
		builder.HasIndex(u => u.PublicId).IsUnique();

		builder.HasData(
			new Picture
			{
				Id = new Guid("11111111-1111-410c-bc78-2d54a9991870"),
				PublicId = "default-profile-picture",
				Url = "https://res.cloudinary.com/dkhfoludi/image/upload/v1749659073/default-profile-picture.jpg"
			}
		);
	}
}

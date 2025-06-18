using Microsoft.AspNetCore.Identity;
using UserService.Domain.Common.Constants;

namespace UserService.Domain.Users;

public class User : IdentityUser<Guid>
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
	public bool IsBlocked { get; private set; }
	public Guid? PictureId { get; set; }
	public virtual ICollection<UserRole> UserRoles { get; set; } = [];

	public User(string userName, string email) : base(userName)
	{
		PictureId = Guid.Parse(Pictures.DefaultId);
		Email = email;
		NormalizedUserName = userName?.ToUpperInvariant();
		NormalizedEmail = email?.ToUpperInvariant();
	}

	public void Block()
	{
		IsBlocked = true;
	}

	public void Unblock()
	{
		IsBlocked = false;
	}
}

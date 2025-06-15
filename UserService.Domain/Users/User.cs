using Microsoft.AspNetCore.Identity;

namespace UserService.Domain.Users;

public class User : IdentityUser<Guid>
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	
	public Picture Picture { get; set; }

	public string? DisplayName => string
		.IsNullOrWhiteSpace($"{FirstName} {LastName}") 
			? UserName 
			: $"{FirstName} {LastName}"
		.Trim();

	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
	public bool IsBlocked { get; private set; }

	public User(string userName, string email) : base(userName)
	{
		Picture = new Picture();
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

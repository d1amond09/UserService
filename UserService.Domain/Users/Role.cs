﻿using Microsoft.AspNetCore.Identity;

namespace UserService.Domain.Users;

public class Role : IdentityRole<Guid> 
{
	public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

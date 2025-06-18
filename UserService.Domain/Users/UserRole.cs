using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserService.Domain.Users;

public class UserRole : IdentityUserRole<Guid>
{
	public virtual User? User { get; set; }
	public virtual Role? Role { get; set; }
}

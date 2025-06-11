using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;
using UserService.Infrastructure.Users.Persistence;

namespace UserService.Infrastructure.Common.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RoleConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await base.SaveChangesAsync(cancellationToken);
	}

	public override int SaveChanges()
	{
		return base.SaveChanges();
	}

}

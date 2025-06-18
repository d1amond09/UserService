using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;
using UserService.Infrastructure.Users.Persistence.Configurations;

namespace UserService.Infrastructure.Common.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
	: IdentityDbContext<
		User, Role, Guid, IdentityUserClaim<Guid>, 
		UserRole, IdentityUserLogin<Guid>, 
		IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
	public DbSet<Picture> Pictures { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new PictureConfiguration());
		modelBuilder.ApplyConfiguration(new RoleConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());

		modelBuilder.Entity<UserRole>(userRole =>
		{
			userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
		});
	}

	public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
	{
		return await base.SaveChangesAsync(ct);
	}
}

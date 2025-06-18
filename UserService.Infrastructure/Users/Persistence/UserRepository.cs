using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;
using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.RequestFeatures;
using UserService.Infrastructure.Common.Persistence;
using Microsoft.AspNetCore.Identity;
using UserService.Infrastructure.Common.Persistence.Extensions;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Infrastructure.Users.Persistence;

public class UserRepository(AppDbContext db) : RepositoryBase<User>(db), IUserRepository
{
	public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct) =>
		await FindAll()
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.FirstOrDefaultAsync(u => u.Id == userId, ct);

	public async Task<PagedList<User>> GetAllAsync(UserParameters userParams, CancellationToken ct)
	{
		IQueryable<User> usersQuery = FindAll()
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.FilterUsers(userParams)
			.Search(userParams.SearchTerm)
			.Sort(userParams.OrderBy);

		return await PagedList<User>.ToPagedListAsync(
			usersQuery,
			userParams.PageNumber,
			userParams.PageSize, 
			ct);
	}

	public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct) =>
		await FindAll()
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.Where(u => userIds.Contains(u.Id))
			.ToListAsync(ct);
}

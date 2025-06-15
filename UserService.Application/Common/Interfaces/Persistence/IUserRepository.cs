using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.RequestFeatures;
using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
	IQueryable<User> GetUsersAsQueryable();
	Task<PagedList<User>> GetUsersAsync(UserParameters userParameters, CancellationToken cancellationToken = default);
	void Update(User user);
}

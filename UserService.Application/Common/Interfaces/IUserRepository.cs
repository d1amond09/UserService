using UserService.Domain.Common.RequestFeatures.ModelParameters;
using UserService.Domain.Common.RequestFeatures;
using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces;

public interface IUserRepository
{
	IQueryable<User> GetUsersAsQueryable();
	Task<PagedList<User>> GetUsersAsync(UserParameters userParameters, CancellationToken cancellationToken = default);
	Task<User?> GetByIdWithNavigationsAsync(Guid id, CancellationToken cancellationToken = default);
	Task<List<User>> SearchUsersForAutocompleteAsync(string searchTerm, int maxResults, CancellationToken cancellationToken = default);
	Task<User?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
	void Update(User user);
}

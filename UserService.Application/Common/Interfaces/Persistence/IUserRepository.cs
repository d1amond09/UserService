using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.RequestFeatures;
using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
	Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct);
	Task<PagedList<User>> GetAllAsync(UserParameters userParams, CancellationToken ct = default);
}

namespace UserService.Application.Common.Interfaces;

public interface IRepositoryManager
{
	IUserRepository Users { get; }

	Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

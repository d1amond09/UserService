namespace UserService.Application.Common.Interfaces.Persistence;

public interface IRepositoryManager
{
	IUserRepository Users { get; }
	IPictureRepository Pictures { get; }

	Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

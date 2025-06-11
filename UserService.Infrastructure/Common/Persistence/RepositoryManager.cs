using UserService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using UserService.Infrastructure.Users.Persistence;

namespace UserService.Infrastructure.Common.Persistence;

public class RepositoryManager : IRepositoryManager
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<User> _userManager;

	private readonly Lazy<IUserRepository> _userRepository;

	public RepositoryManager(AppDbContext dbContext, UserManager<User> userManager)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

		_userRepository = new Lazy<IUserRepository>(() => new UserRepository(_userManager, _dbContext));
	}

	public IUserRepository Users => _userRepository.Value;

	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
	{
		return await _dbContext.SaveChangesAsync(cancellationToken);
	}

	private bool disposed = false;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				_dbContext.Dispose(); 
			}
		}
		disposed = true;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}

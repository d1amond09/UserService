using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using UserService.Infrastructure.Users.Persistence;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Infrastructure.Common.Persistence;

public class RepositoryManager : IRepositoryManager
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<User> _userManager;
	private readonly Lazy<IPictureRepository> _pictureRepository;
	private readonly Lazy<IUserRepository> _userRepository;

	public RepositoryManager(AppDbContext dbContext, UserManager<User> userManager)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

		_userRepository = new Lazy<IUserRepository>(() => new UserRepository(_userManager, _dbContext));
		_pictureRepository = new Lazy<IPictureRepository>(() => new PictureRepository(_dbContext));
	}

	public IUserRepository Users => _userRepository.Value;
	public IPictureRepository Pictures => _pictureRepository.Value;

	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
	{
		return await _dbContext.SaveChangesAsync(cancellationToken);
	}
}

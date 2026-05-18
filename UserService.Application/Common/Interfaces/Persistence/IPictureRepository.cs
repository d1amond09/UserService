using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces.Persistence;

public interface IPictureRepository
{
	Task<Picture?> GetByIdAsync(Guid id);
	Task<Picture?> GetDefautPictureAsync();
	Task SaveAsync(CancellationToken ct);
	void Create(Picture picture);
	void Delete(Picture picture);
}

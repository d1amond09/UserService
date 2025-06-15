using UserService.Domain.Users;

namespace UserService.Application.Common.Interfaces.Persistence;

public interface IPictureRepository
{
	Task<Picture?> GetDefautPictureAsync();
}

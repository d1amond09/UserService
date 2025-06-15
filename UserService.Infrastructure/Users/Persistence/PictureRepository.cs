using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.RequestFeatures;
using UserService.Infrastructure.Common.Persistence;
using Microsoft.AspNetCore.Identity;
using UserService.Infrastructure.Common.Persistence.Extensions;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Domain.Common.Constants;

namespace UserService.Infrastructure.Users.Persistence;

public class PictureRepository(AppDbContext db) : RepositoryBase<Picture>(db), IPictureRepository
{
	public async Task<Picture?> GetDefautPictureAsync() => 
		await FindByCondition(p => p.Id
			.Equals(Guid.Parse(Pictures.DefaultId)), false)
		.SingleOrDefaultAsync();


}

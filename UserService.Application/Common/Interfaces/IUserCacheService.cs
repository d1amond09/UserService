namespace UserService.Application.Common.Interfaces;

public interface IUserCacheService
{
	Task InvalidateUserCacheAsync(Guid userId, CancellationToken ct);
}

using MediatR;

namespace UserService.Application.Common.Notifications;

public class CacheInvalidationNotification(string cacheKey) : INotification
{
	public string CacheKey { get; } = cacheKey;
}

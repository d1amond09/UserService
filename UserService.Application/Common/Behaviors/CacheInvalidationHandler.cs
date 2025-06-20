using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Notifications;

namespace UserService.Application.Common.Behaviors;

public class CacheInvalidationHandler(
	IDistributedCache cache,
	ILogger<CacheInvalidationHandler> logger)
	: INotificationHandler<CacheInvalidationNotification>
{
	public async Task Handle(CacheInvalidationNotification notification, CancellationToken ct)
	{
		try
		{
			await cache.RemoveAsync(notification.CacheKey, ct);
			logger.LogInformation("Cache invalidated for key: {CacheKey}", notification.CacheKey);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred during cache invalidation for key: {CacheKey}", notification.CacheKey);
		}
	}
}

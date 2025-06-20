using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using UserService.Application.Common.Interfaces;
using UserService.Application.UseCases.Users.GetUserById;

namespace UserService.Infrastructure.Services;

public class UserCacheService(IDistributedCache cache) : IUserCacheService
{
	public Task InvalidateUserCacheAsync(Guid userId, CancellationToken ct)
	{
		var queryToInvalidate = new GetUserByIdQuery(userId);

		string cacheKey = GenerateCacheKey(queryToInvalidate);

		return cache.RemoveAsync(cacheKey, ct);
	}

	private string GenerateCacheKey<TRequest>(TRequest request)
	{
		var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = false });
		return $"{request?.GetType().Name}:{requestJson}";
	}
}

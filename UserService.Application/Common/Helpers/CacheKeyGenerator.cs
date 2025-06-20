using System.Text.Json;
using UserService.Application.UseCases.Users.GetUserById;

namespace UserService.Application.Common.Helpers;

public static class CacheKeyGenerator
{
	public static string GetUserCacheKey(Guid userId)
	{
		var query = new GetUserByIdQuery(userId);
		var requestJson = JsonSerializer.Serialize(query, new JsonSerializerOptions { WriteIndented = false });
		return $"{query.GetType().Name}:{requestJson}";
	}
}

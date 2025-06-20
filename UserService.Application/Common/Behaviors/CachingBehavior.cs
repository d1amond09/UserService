using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Text.Json;
using System.Text;
using UserService.Application.Common.Attributes;

namespace UserService.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(IDistributedCache cache)
	: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		var cacheableAttribute = request.GetType().GetCustomAttribute<CacheableAttribute>();
		if (cacheableAttribute is null)
		{
			return await next(ct);
		}

		string cacheKey = GenerateCacheKey(request);

		var cachedResponse = await cache.GetAsync(cacheKey, ct);
		if (cachedResponse != null)
		{
			var json = Encoding.UTF8.GetString(cachedResponse);
			return JsonSerializer.Deserialize<TResponse>(json)!;
		}

		var response = await next(ct);

		var options = new DistributedCacheEntryOptions()
			.SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheableAttribute.ExpirationMinutes));

		var jsonResponse = JsonSerializer.Serialize(response);
		var bytes = Encoding.UTF8.GetBytes(jsonResponse);

		await cache.SetAsync(cacheKey, bytes, options, ct);

		return response;
	}

	private string GenerateCacheKey(TRequest request)
	{
		var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = false });
		return $"{request.GetType().Name}:{requestJson}";
	}
}

namespace UserService.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)] 
public class CacheableAttribute(int expirationMinutes = 5) : Attribute
{
	public int ExpirationMinutes { get; } = expirationMinutes;
}

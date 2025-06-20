using UserService.Application.Common;
using UserService.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
		services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
		
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

		return services;
	}
}

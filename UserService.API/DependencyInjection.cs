using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using UserService.API.ExceptionHandlers;

namespace UserService.API;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(
		this IServiceCollection services,
		IConfiguration config,
		IWebHostEnvironment environment)
	{
		services.AddProblemDetailsAsExceptionHandler(config, environment);
		services.AddSwagger();
		services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder => 
				builder.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod()
					.WithExposedHeaders("X-Pagination"));
		});

		services.AddControllers()
			.AddJsonOptions(opts => 
				opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

		services.AddEndpointsApiExplorer();

		return services;
	}

	public static IServiceCollection AddProblemDetailsAsExceptionHandler(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
	{
		services.AddExceptionHandler<BadRequestExceptionHandler>();
		services.AddExceptionHandler<ForbiddenAccessExceptionHandler>();
		services.AddExceptionHandler<NotFoundExceptionHandler>();

		services.AddProblemDetails(opts => 
		{
			opts.CustomizeProblemDetails = (pdContext) =>
			{
				var ex = pdContext.Exception;

				pdContext.ProblemDetails.Extensions["traceId"] = pdContext.HttpContext.TraceIdentifier;

				if (ex is not null)
					pdContext.ProblemDetails.Extensions["exception"] = ex.ToString();
			};
		});

		return services;
	}

	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(s =>
		{
			s.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Custom Forms API",
				Version = "v1"
			});
			s.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

			s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Place to add JWT with Bearer",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});
			s.AddSecurityRequirement(new OpenApiSecurityRequirement() { {
				new OpenApiSecurityScheme {
					Reference = new OpenApiReference {
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					},
					Name = "Bearer",
				},
				new List<string>()
			} });
		});

		return services;
	}

}

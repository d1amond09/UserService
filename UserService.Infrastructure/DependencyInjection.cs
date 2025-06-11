using System.Configuration;
using System.Diagnostics;
using System.Text;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using UserService.Infrastructure.Common.Persistence;
using UserService.Infrastructure.Security.CurrentUserService;
using UserService.Infrastructure.Security.TokenGenerator;
using UserService.Infrastructure.Security.TokenValidation;
using UserService.Infrastructure.Users.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddHttpContextAccessor()
			.AddServices(configuration)
			.AddAuthorization()
			.AddConfigIdentity()
			.AddAuthentication(configuration)
			.AddPersistence(configuration);

		return services;
	}

	private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IDataShapeService<UserDtos>, DataShapeService<UserDtos>>();
		services.AddScoped<ICloudinaryService, CloudinaryService>();

		return services;
	}

	private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		string connectionString = configuration.GetConnectionString("Default") ?? string.Empty;

		services.AddDbContext<AppDbContext>(opts =>
			opts.UseNpgsql(connectionString, b =>
			{
				b.MigrationsAssembly("UserService.Infrastructure");
				b.EnableRetryOnFailure();
			})
		);

		services.AddScoped<IRepositoryManager, RepositoryManager>();

		return services;
	}

	private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddSingleton<ITokenValidator, TokenValidator>();
		services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

		var jwtSettings = configuration.GetSection("JwtSettings");
		var secretKey = configuration.GetValue<string>("SECRET");
		ArgumentNullException.ThrowIfNull(secretKey);

		services
		.AddAuthentication(opt =>
		{
			opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,

				ValidIssuer = jwtSettings["validIssuer"],
				ValidAudience = jwtSettings["validAudience"],

				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
			};
		});

		return services;
	}


	public static IServiceCollection AddConfigIdentity(this IServiceCollection services)
	{
		var builder = services.AddIdentity<User, Role>(o =>
		{
			o.Password.RequireDigit = true;
			o.Password.RequireLowercase = true;
			o.Password.RequireUppercase = true;
			o.Password.RequireNonAlphanumeric = true;
			o.Password.RequiredLength = 8;
			o.User.RequireUniqueEmail = true;
		})
		.AddEntityFrameworkStores<AppDbContext>()
		.AddDefaultTokenProviders();

		return services;
	}
}

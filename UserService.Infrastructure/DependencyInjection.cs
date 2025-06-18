using System.Configuration;
using System.Diagnostics;
using System.Text;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using UserService.Infrastructure.Common.Persistence;
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
using UserService.Application.Common.Interfaces.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserService.Infrastructure.Security;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, 
		IConfiguration configuration)
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

		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IPictureRepository, PictureRepository>();

		return services;
	}

	private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddScoped<ITokenService, TokenService>();

		var jwtSettings = new JwtSettings();
		configuration.Bind(JwtSettings.SectionName, jwtSettings);
		services.AddSingleton(Options.Create(jwtSettings));

		services
		.AddAuthentication(opts =>
		{
			opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(opts =>
		{
			opts.TokenValidationParameters = TokenValidationParametersFactory.Create(
				jwtSettings,
				validateLifetime: true
			);
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

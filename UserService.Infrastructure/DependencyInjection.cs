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
using UserService.Infrastructure.Common.Configuration;
using UserService.Infrastructure.Security.CustomTokenProviders;
using Microsoft.AspNetCore.Http;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, 
		IConfiguration config)
	{
		services
			.AddHttpContextAccessor()
			.AddConfigurations(config)
			.AddServices()
			.AddAuthorization()
			.AddConfigIdentity()
			.AddAuthentication(config)
			.AddPersistence(config);

		return services;
	}

	private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
	{
		services.Configure<WebAppSettings>(config.GetSection(WebAppSettings.SectionName));
		services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
		services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));
		return services;
	}

	private static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddScoped<ICloudinaryService, CloudinaryService>();
		services.AddScoped<IEmailService, SmtpEmailService>();
		services.AddScoped<IExternalAuthService, GoogleAuthService>();

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

		var googleAuthSettings = new GoogleAuthSettings();
		configuration.GetSection($"Authentication:{GoogleAuthSettings.SectionName}").Bind(googleAuthSettings);
		services.AddSingleton(Options.Create(googleAuthSettings));

		var jwtSettings = new JwtSettings();
		configuration.Bind(JwtSettings.SectionName, jwtSettings);
		services.AddSingleton(Options.Create(jwtSettings));

		var authBuilder = services
		.AddAuthentication(opts =>
		{
			opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		});
		authBuilder.AddJwtBearer(opts =>
		{
			opts.TokenValidationParameters = TokenValidationParametersFactory.Create(
				jwtSettings,
				validateLifetime: true
			);
		});
		authBuilder.AddGoogle(options =>
		{
			options.ClientId = googleAuthSettings.ClientId;
			options.ClientSecret = googleAuthSettings.ClientSecret;
			options.CallbackPath = "/signin-google";
			options.Scope.Add("profile");
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
			o.SignIn.RequireConfirmedEmail = true;
			o.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
		})
		.AddEntityFrameworkStores<AppDbContext>()
		.AddDefaultTokenProviders()
		.AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation")
		.AddSignInManager<CustomSignInManager>();

		services.Configure<DataProtectionTokenProviderOptions>(opt =>
			opt.TokenLifespan = TimeSpan.FromHours(2));

		services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
			opt.TokenLifespan = TimeSpan.FromDays(3));

		return services;
	}
}

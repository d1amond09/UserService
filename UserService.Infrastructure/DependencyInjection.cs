using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Domain.Users;
using UserService.Infrastructure.Common.Configuration;
using UserService.Infrastructure.Common.Persistence;
using UserService.Infrastructure.Security;
using UserService.Infrastructure.Security.CustomTokenProviders;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Users.Persistence;
using Role = UserService.Domain.Users.Role;

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
			.AddCacheRedis(config)
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
		services.Configure<CloudinarySettings>(config.GetSection(CloudinarySettings.SectionName));
		services.Configure<CacheSettings>(config.GetSection(CacheSettings.SectionName));

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

	private static IServiceCollection AddCacheRedis(this IServiceCollection services, IConfiguration configuration)
	{
		var cacheSettings = new CacheSettings();
		configuration.Bind(CacheSettings.SectionName, cacheSettings);
		services.AddSingleton(Options.Create(cacheSettings));

		services.AddSingleton<IConnectionMultiplexer>(sp =>
		{
			var options = new ConfigurationOptions
			{
				EndPoints = { { cacheSettings.Host, cacheSettings.Port } },
				User = cacheSettings.User,
				Password = cacheSettings.Password, 
				AbortOnConnectFail = false
			};
			return ConnectionMultiplexer.Connect(options);
		});

		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = $"{cacheSettings.Host}:{cacheSettings.Port},user={cacheSettings.User},password={cacheSettings.Password}";
			options.InstanceName = cacheSettings.InstanceName;
		});

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

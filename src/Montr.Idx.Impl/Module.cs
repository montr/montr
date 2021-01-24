using System;
using System.Linq;
using System.Security.Cryptography;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;
using Montr.Idx.Models;
using Montr.Idx.Services;
using OpenIddict.Abstractions;

namespace Montr.Idx.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		private readonly IHostEnvironment _environment;

		public Module(IHostEnvironment environment)
		{
			_environment = environment;
		}

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, RegisterMessageTemplateStartupTask>();

			services.AddTransient<IEmailConfirmationService, EmailConfirmationService>();
			services.AddTransient<IRepository<User>, DbUserRepository>();
			services.AddTransient<IUserManager, DefaultUserManager>();
			services.AddTransient<ISignInManager, DefaultSignInManager>();

			// todo: move from impl to idx?
			services.Configure<IdentityOptions>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;

				options.User.RequireUniqueEmail = true;
				// options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;
			});

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql(configuration.GetConnectionString("Default"));

				// Register the entity sets needed by OpenIddict.
				// Note: use the generic overload if you need
				// to replace the default OpenIddict entities.
				options.UseOpenIddict();
			});

			IdentitySchemaMapper.MapSchema(MappingSchema.Default);

			services
				.AddIdentity<DbUser, DbRole>()
				.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
				.AddLinqToDBStores(new DbConnectionFactory(), // todo: why connection factory instance here?
					typeof(Guid),
					typeof(LinqToDB.Identity.IdentityUserClaim<Guid>),
					typeof(LinqToDB.Identity.IdentityUserRole<Guid>),
					typeof(LinqToDB.Identity.IdentityUserLogin<Guid>),
					typeof(LinqToDB.Identity.IdentityUserToken<Guid>),
					typeof(LinqToDB.Identity.IdentityRoleClaim<Guid>))
				.AddDefaultTokenProviders();

			/*services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = PathString.Empty;
				options.LoginPath = "/account/login";
				options.LogoutPath = "/account/logout";
				options.AccessDeniedPath = "/account/access-denied";

				options.Events = new CookieAuthenticationEvents()
				{
					OnRedirectToLogin = (ctx) =>
					{
						if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
						{
							ctx.Response.StatusCode = 401;
						}

						return Task.CompletedTask;
					},
					OnRedirectToAccessDenied = (ctx) =>
					{
						if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
						{
							ctx.Response.StatusCode = 403;
						}

						return Task.CompletedTask;
					}
				};
			});*/

			// services.AddOpenIdAuthentication(configuration.GetSection("OpenId").Get<OpenIdOptions>());

			/* services.AddAuthentication(options =>
				{
					// x.DefaultAuthenticateScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
					x.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
					x.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
					x.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;

					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				}) */

			// .AddIdentityServerAuthentication()
			// .AddJwtBearer()

			// todo: use IOptions
			var appOptions = configuration.GetOptions<AppOptions>();

			services.AddOpenIddict()

				// Register the OpenIddict core components.
				.AddCore(options =>
				{
					// Configure OpenIddict to use the Entity Framework Core stores and models.
					// Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
					options.UseEntityFrameworkCore()
						.UseDbContext<ApplicationDbContext>();
				})

				// Register the OpenIddict server components.
				.AddServer(options =>
				{
					// Enable the authorization, logout, token and userinfo endpoints.
					options.SetAuthorizationEndpointUris("/connect/authorize")
						.SetLogoutEndpointUris("/connect/logout")
						.SetTokenEndpointUris("/connect/token")
						.SetUserinfoEndpointUris("/connect/userinfo");

					// Mark the "email", "profile" and "roles" scopes as supported scopes.
					options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles);

					// Note: the sample uses the code and refresh token flows but you can enable
					// the other flows if you need to support implicit, password or client credentials.
					options.AllowAuthorizationCodeFlow()
						.AllowRefreshTokenFlow();

					// Enable the client credentials flow.
					options.AllowClientCredentialsFlow();

					// Register the signing and encryption credentials.
					options.AddDevelopmentEncryptionCertificate()
						.AddDevelopmentSigningCertificate();

					// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
					options.UseAspNetCore()
						.EnableAuthorizationEndpointPassthrough()
						.EnableLogoutEndpointPassthrough()
						.EnableStatusCodePagesIntegration()
						.EnableTokenEndpointPassthrough();
				})

				// Register the OpenIddict validation components.
				.AddValidation(options =>
				{
					// Import the configuration from the local OpenIddict server instance.
					options.UseLocalServer();

					// Register the ASP.NET Core host.
					options.UseAspNetCore();
				});

			// Register the worker responsible of seeding the database with the sample clients.
			// Note: in a real world application, this step should be part of a setup script.
			services.AddHostedService<Worker>();

			// todo: move to Montr.Idx.Plugin.IdentityServer
			/*var builder = services
				.AddIdentityServer(options =>
				{
					// options.PublicOrigin = appOptions.AppUrl; // todo: missing in dotnet 5.0
					// options.Authentication.CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;

					options.Cors.CorsPolicyName = AppConstants.CorsPolicyName;

					options.UserInteraction.LoginUrl = ClientRoutes.Login;
					options.UserInteraction.LogoutUrl = ClientRoutes.Logout;
				})
				// https://www.scottbrady91.com/Identity-Server/Creating-Your-Own-IdentityServer4-Storage-Library
				// https://damienbod.com/2017/12/30/using-an-ef-core-database-for-the-identityserver4-configuration-data/
				// .AddPersistedGrantStore<>()
				.AddInMemoryPersistedGrants()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients(appOptions.ClientUrls))
				.AddAspNetIdentity<DbUser>();

			if (_environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential(); // tempkey.rsa
			}
			else
			{
				// todo: use one certificate for all instances
				builder.AddSigningCredential(new SigningCredentials(
					new RsaSecurityKey(RSA.Create(2048)), SecurityAlgorithms.RsaSha256Signature
				));
			}*/
		}

		public void Configure(IApplicationBuilder app)
		{
			// app.UseCors("default"); // not needed, since UseIdentityServer adds cors
			// app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
			// app.UseIdentityServer();

			app.UseAuthentication();
			app.UseAuthorization();
		}
	}
}

using System;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;
using Montr.Idx.Services;
using Montr.MasterData.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Montr.Idx.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, RegisterPermissionsStartupTask>();

			services.AddSingleton<IContentProvider, ContentProvider>();
			services.AddTransient<IPermissionProvider, PermissionProvider>();

			services.AddTransient<IEmailConfirmationService, EmailConfirmationService>();
			services.AddTransient<ISignInManager, DefaultSignInManager>();
			services.AddTransient<IOidcServer, OpenIddictServer>();

			services.AddNamedTransient<IClassifierRepository, DbRoleRepository>(ClassifierTypeCode.Role);
			services.AddNamedTransient<IClassifierRepository, DbUserRepository>(ClassifierTypeCode.User);

			services.AddScoped<IAuthorizationHandler, UserPermissionAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, SuperUserAuthorizationHandler>();

			services
				.AddAuthentication()
				.AddCookie();

			// todo: move from impl to idx?
			services.Configure<IdentityOptions>(options =>
			{
				// todo: move to settings
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;

				options.User.RequireUniqueEmail = true;

				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				options.ClaimsIdentity.UserNameClaimType = Claims.Name;
				options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
				options.ClaimsIdentity.RoleClaimType = Claims.Role;
				options.ClaimsIdentity.EmailClaimType = Claims.Email;
			});

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				// todo: remove hardcoded connection name
				options.UseNpgsql(configuration.GetConnectionString("Default"));

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

			// ConfigureApplicationCookie should be after AddIdentity
			// https://github.com/dotnet/aspnetcore/issues/5828
			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = ClientRoutes.Login;
				options.LogoutPath = ClientRoutes.Logout;
				// options.AccessDeniedPath = "/account/access-denied";

				/*options.Events = new CookieAuthenticationEvents
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
				};*/
			});

			services.AddOpenIddict()
				.AddCore(options =>
				{
					options.UseEntityFrameworkCore()
						.UseDbContext<ApplicationDbContext>();
				})

				// Register the OpenIddict server components.
				.AddServer(options =>
				{
					// Enable the authorization, logout, token and userinfo endpoints.
					options
						.SetAuthorizationEndpointUris("/connect/authorize")
						.SetLogoutEndpointUris("/connect/logout")
						.SetTokenEndpointUris("/connect/token")
						.SetUserinfoEndpointUris("/connect/userinfo");

					// Mark the "email", "profile" and "roles" scopes as supported scopes.
					options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

					options
						.AllowAuthorizationCodeFlow()
						.AllowRefreshTokenFlow()
						.AllowClientCredentialsFlow()
						.AllowImplicitFlow();

					// Register the signing and encryption credentials.
					options.AddDevelopmentEncryptionCertificate()
						.AddDevelopmentSigningCertificate();

					// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
					options.UseAspNetCore()
						.EnableAuthorizationEndpointPassthrough()
						.EnableTokenEndpointPassthrough()
						.EnableLogoutEndpointPassthrough()
						// .EnableUserinfoEndpointPassthrough()
						.EnableStatusCodePagesIntegration();
				})

				// Register the OpenIddict validation components.
				.AddValidation(options =>
				{
					// Import the configuration from the local OpenIddict server instance.
					options.UseLocalServer();

					// Register the ASP.NET Core host.
					options.UseAspNetCore();
				});

			services.AddAuthorization();

			// Register the worker responsible of seeding the database with the sample clients.
			// Note: in a real world application, this step should be part of a setup script.
			services.AddHostedService<Worker>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseCors(AppConstants.CorsPolicyName);
			app.UseAuthentication();
			app.UseAuthorization();
		}
	}
}

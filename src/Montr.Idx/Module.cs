using System;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Entities;
using Montr.Idx.Services;
using Montr.Idx.Services.Implementations;
using Montr.MasterData.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Montr.Idx
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<SignInSettings>(appBuilder.Configuration);
			appBuilder.Services.BindOptions<PasswordSettings>(appBuilder.Configuration);
			appBuilder.Services.BindOptions<LockoutSettings>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, RegisterMessageTemplateStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, RegisterPermissionsStartupTask>();

			appBuilder.Services.AddTransient<IConfigureOptions<IdentityOptions>, IdentityOptionsConfigurator>();

			appBuilder.Services.AddTransient<IContentProvider, ContentProvider>();
			appBuilder.Services.AddTransient<IPermissionProvider, IdxPermissionProvider>();

			appBuilder.Services.AddTransient<IEmailConfirmationService, EmailConfirmationService>();
			appBuilder.Services.AddTransient<ISignInManager, DefaultSignInManager>();
			appBuilder.Services.AddTransient<IOidcServer, OpenIddictServer>();

			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbRoleRepository>(ClassifierTypeCode.Role);
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbUserRepository>(ClassifierTypeCode.User);

			appBuilder.Services.AddScoped<IAuthorizationHandler, SuperUserAuthorizationHandler>();
			appBuilder.Services.AddScoped<IAuthorizationHandler, UserPermissionAuthorizationHandler>();

			appBuilder.Services
				.AddAuthentication()
				.AddCookie();

			appBuilder.Services.Configure<IdentityOptions>(options =>
			{
				// note: other settings are configured in ui and loaded in IdentityOptionsConfigurator

				options.User.RequireUniqueEmail = true;

				options.ClaimsIdentity.UserNameClaimType = Claims.Name;
				options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
				options.ClaimsIdentity.RoleClaimType = Claims.Role;
				options.ClaimsIdentity.EmailClaimType = Claims.Email;
			});

			appBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				// todo: remove hardcoded connection name
				options.UseNpgsql(appBuilder.Configuration.GetConnectionString("Default"));

				options.UseOpenIddict();
			});

			IdentitySchemaMapper.MapSchema(MappingSchema.Default);

			appBuilder.Services
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
			appBuilder.Services.ConfigureApplicationCookie(options =>
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

			// todo: move OpenIddict to separate module
			appBuilder.Services.AddOpenIddict()
				.AddCore(options =>
				{
					options.UseEntityFrameworkCore()
						.UseDbContext<ApplicationDbContext>();
				})

				// Register the OpenIddict server components.
				.AddServer(options =>
				{
					// Enable the authorization, logout, token and userinfo endpoints.
					options.SetAuthorizationEndpointUris("connect/authorize")
						// .SetDeviceEndpointUris("connect/device")
						.SetIntrospectionEndpointUris("connect/introspect")
						.SetLogoutEndpointUris("connect/logout")
						.SetTokenEndpointUris("connect/token")
						.SetUserinfoEndpointUris("connect/userinfo")
						.SetVerificationEndpointUris("connect/verify");

					// Mark the "email", "profile" and "roles" scopes as supported scopes.
					options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

					options
						.AllowAuthorizationCodeFlow()
						.AllowRefreshTokenFlow()
						.AllowClientCredentialsFlow()
						.AllowImplicitFlow();

					// Register the signing and encryption credentials.
					options
						.AddDevelopmentEncryptionCertificate()
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

			appBuilder.Services.AddAuthorization();

			// Register the worker responsible of seeding the database with the sample clients.
			// Note: in a real world application, this step should be part of a setup script.
			appBuilder.Services.AddHostedService<Worker>();
		}

		public void Configure(IApp app)
		{
			app.UseCors(AppConstants.CorsPolicyName);
			app.UseAuthentication();
			app.UseAuthorization();
		}
	}
}

using System;
using System.Security.Cryptography;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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
			services.BindOptions<Options>(configuration);

			services.AddTransient<IStartupTask, CreateDefaultAdminStartupTask>();
			services.AddTransient<IStartupTask, RegisterMessageTemplateStartupTask>();

			services.AddTransient<EmailConfirmationService, EmailConfirmationService>();
			services.AddTransient<IRepository<User>, DbUserRepository>();

			// todo: move from impl to idx?
			services.Configure<IdentityOptions>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;

				options.User.RequireUniqueEmail = false;
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

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

			// todo: move to Montr.Idx.Plugin.IdentityServer
			var builder = services
				.AddIdentityServer(options =>
				{
					options.PublicOrigin = appOptions.AppUrl;
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
			}
		}

		public void Configure(IApplicationBuilder app)
		{
			// app.UseCors("default"); // not needed, since UseIdentityServer adds cors
			// app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
			app.UseIdentityServer();
			app.UseAuthorization();
		}
	}
}

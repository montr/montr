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
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

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
			services.AddTransient<EmailConfirmationService, EmailConfirmationService>();

			// todo: move from impl?
			services.Configure<IdentityOptions>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = true;
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
				.AddLinqToDBStores(new DbConnectionFactory(),
					typeof(Guid),
					typeof(LinqToDB.Identity.IdentityUserClaim<Guid>),
					typeof(LinqToDB.Identity.IdentityUserRole<Guid>),
					typeof(LinqToDB.Identity.IdentityUserLogin<Guid>),
					typeof(LinqToDB.Identity.IdentityUserToken<Guid>),
					typeof(LinqToDB.Identity.IdentityRoleClaim<Guid>))
				.AddDefaultTokenProviders();

			var idxServerOptions = configuration.GetSection("IdxServer").Get<IdxServerOptions>();

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

			var builder = services
				.AddIdentityServer(options =>
				{
					options.PublicOrigin = idxServerOptions.PublicOrigin;
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
				.AddInMemoryClients(Config.GetClients(idxServerOptions.ClientUrls))
				.AddAspNetIdentity<DbUser>();

			/*services.AddOpenIdApiAuthentication(
				configuration.GetSection("OpenId").Get<OpenIdOptions>());*/

			services
				.AddAuthentication()
				// .AddIdentityServerJwt()
				.AddGoogle("Google", options =>
				{
					// options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					options.SignInScheme = IdentityConstants.ExternalScheme;

					options.ClientId = configuration["Authentication:Google:ClientId"];
					options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
				})
				.AddFacebook(options =>
				{
					options.AppId = configuration["Authentication:Facebook:AppId"];
					options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
				});

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
			// app.UseAuthorization();
		}
	}
}

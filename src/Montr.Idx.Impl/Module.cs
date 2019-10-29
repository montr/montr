using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4;
using LinqToDB.Identity;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;
using Montr.Modularity;

namespace Montr.Idx.Impl
{
	public class Module : IModule
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

			// https://github.com/linq2db/linq2db/issues/286
			// https://github.com/linq2db/t4models
			MapSchema(MappingSchema.Default);

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

					options.Cors.CorsPolicyName = "default";

					options.UserInteraction.LoginUrl = "/account/login";
					options.UserInteraction.LogoutUrl = "/account/logout";
				})
				// https://www.scottbrady91.com/Identity-Server/Creating-Your-Own-IdentityServer4-Storage-Library
				// https://damienbod.com/2017/12/30/using-an-ef-core-database-for-the-identityserver4-configuration-data/
				// .AddPersistedGrantStore<>()
				.AddInMemoryPersistedGrants()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients(idxServerOptions.ClientUrls))
				// .AddApiAuthorization<DbUser, ApiAuthorizationDbContext<DbUser>>()
				.AddAspNetIdentity<DbUser>();

			services
				// .AddAuthentication("JwtBearer")
				/*.AddAuthentication(options =>
				{
					// https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2321
					options.DefaultAuthenticateScheme = "JwtBearer";
					options.DefaultChallengeScheme = "JwtBearer";
				})*/
				// .AddJwtAuthentication()
				/*.AddAuthentication(options =>
				{
					// options.DefaultScheme = IdentityServerConstants.JwtRequestClientKey;
				})*/
				// .AddIdentityServerJwt()
				/*.AddJwtBearer(options =>
				{
					// base-address of your identityserver
					options.Authority = idxServerOptions.PublicOrigin;

					// name of the API resource
					options.Audience = "tendr";
				})*/
				/*.AddAuthentication(options =>
				{
					// x.DefaultAuthenticateScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
					// x.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
					// x.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
					// x.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
				})*/
				.AddAuthentication()
				.AddGoogle("Google", options =>
				{
					// options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					options.SignInScheme = IdentityConstants.ExternalScheme;

					options.ClientId = configuration["Authentication:Google:ClientId"];
					options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
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

		public static void MapSchema(MappingSchema mappingSchema)
		{
			mappingSchema.GetFluentMappingBuilder()

				.Entity<LinqToDB.Identity.IdentityRoleClaim<Guid>>().HasTableName("role_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey()
				.Property(x => x.RoleId).HasColumnName("role_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserClaim<Guid>>().HasTableName("user_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey().IsIdentity()
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserLogin<Guid>>().HasTableName("user_login")
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(0)
				.Property(x => x.ProviderKey).HasColumnName("provider_key").IsPrimaryKey(1)
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name")

				.Entity<LinqToDB.Identity.IdentityUserRole<Guid>>().HasTableName("user_role")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.RoleId).HasColumnName("role_id").IsPrimaryKey(1)

				.Entity<LinqToDB.Identity.IdentityUserToken<Guid>>().HasTableName("user_token")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(1)
				.Property(x => x.Name).HasColumnName("name").IsPrimaryKey(2)
				.Property(x => x.Value).HasColumnName("value");
		}
	}
}

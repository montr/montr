using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Idx.Entities;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Idx.Services;
using LinqToDB.Mapping;
using DbContext = Idx.Entities.DbContext;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Idx
{
	public class Startup
	{
		public Startup(IHostingEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
		}

		public IHostingEnvironment Environment { get; }

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			var connectionString = Configuration.GetSection("ConnectionString")["ConnectionString"];

			// Set connection configuration
			DataConnection
				.AddConfiguration(
					"Default",
					connectionString,
					new PostgreSQLDataProvider("Default", PostgreSQLVersion.v95));

			DataConnection.DefaultConfiguration = "Default";
			DbContext.MapSchema(MappingSchema.Default);

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

			services.AddCors(options =>
			{
				options.AddPolicy("default", policy =>
				{
					policy.WithOrigins(
							"http://kompany.montr.io:5010",
							"http://tendr.montr.io:5000",
							"http://app.tendr.montr.io:5000")
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			services.Configure<IdentityOptions>(options =>
			{
				options.User.RequireUniqueEmail = false;

				/* // Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; */
			});

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddRazorPagesOptions(options =>
				{
					options.AllowAreas = true;
					options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				});

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Identity/Account/Login";
				options.LogoutPath = "/Account/Logout";
				options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			});

			// using Microsoft.AspNetCore.Identity.UI.Services;
			services.AddSingleton<IEmailSender, EmailSender>();

			var builder = services.AddIdentityServer(options =>
				{
					options.Authentication.CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;

					options.Cors.CorsPolicyName = "default";

					options.UserInteraction.LoginUrl = "/Identity/Account/Login";
					options.UserInteraction.LogoutUrl = "/Account/Logout";
				})
				// https://www.scottbrady91.com/Identity-Server/Creating-Your-Own-IdentityServer4-Storage-Library
				// https://damienbod.com/2017/12/30/using-an-ef-core-database-for-the-identityserver4-configuration-data/
				// .AddPersistedGrantStore<>()
				.AddInMemoryPersistedGrants()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients())

				.AddAspNetIdentity<DbUser>();

			if (Environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential(); // tempkey.rsa
			}
			else
			{
				// todo: use one certificate for all instances
				builder.AddSigningCredential(new Microsoft.IdentityModel.Tokens.SigningCredentials(
					new RsaSecurityKey(new RSACryptoServiceProvider(2048)), SecurityAlgorithms.RsaSha256Signature
				));
			}

			services
				.AddAuthentication(x =>
				{
					// x.DefaultAuthenticateScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
					x.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
					x.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
					x.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
				})
				.AddGoogle("Google", options =>
				{
					// options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					options.SignInScheme = IdentityConstants.ExternalScheme;

					options.ClientId = Configuration["Authentication:Google:ClientId"];
					options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			/* if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else */
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			// app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			// app.UseCors("default"); // not needed, since UseIdentityServer adds cors
			// app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
			app.UseIdentityServer();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Idx.Data;
using Idx.Entities;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Idx
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

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

			/*services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(connectionString);
			});*/

			// Set connection configuration
			DataConnection
				.AddConfiguration(
					"Default",
					connectionString,
					new PostgreSQLDataProvider("Default", PostgreSQLVersion.v95));
					// new SqlServerDataProvider("Default", SqlServerVersion.v2012));

			DataConnection.DefaultConfiguration = "Default";

			services
				// .AddDefaultIdentity<DbUser>()
				// .AddEntityFrameworkStores<ApplicationDbContext>()
				.AddIdentity<DbUser, DbRole>(options =>
				{
				})
				// .AddLinqToDBStores(new DefaultConnectionFactory())
				.AddLinqToDBStores(new DefaultConnectionFactory(),
					typeof(Guid),
					typeof(LinqToDB.Identity.IdentityUserClaim<Guid>),
					typeof(LinqToDB.Identity.IdentityUserRole<Guid>),
					typeof(LinqToDB.Identity.IdentityUserLogin<Guid>),
					typeof(LinqToDB.Identity.IdentityUserToken<Guid>),
					typeof(LinqToDB.Identity.IdentityRoleClaim<Guid>))
				.AddDefaultTokenProviders()
				.AddDefaultUI();

			/*services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
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
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = false;
			});*/

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// configure identity server with in-memory stores, keys, clients and scopes
			services.AddIdentityServer(options =>
				{
					options.UserInteraction.LoginUrl = "/Identity/Account/Login";
					options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
				})
				.AddDeveloperSigningCredential() // tempkey.rsa
				.AddInMemoryPersistedGrants()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients())
				.AddAspNetIdentity<DbUser>(); // ApplicationUser
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			// app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

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
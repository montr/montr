using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Idx.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Montr.Modularity;

namespace Idx
{
	public class IdxServerOptions
	{
		public string PublicOrigin { get; set; }

		// todo: temp solution, read client configuration from db
		public string[] ClientUrls { get; set; }
	}

	public class Startup
	{
		public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment, IConfiguration configuration)
		{
			Logger = loggerFactory.CreateLogger<Startup>();

			Environment = environment;
			Configuration = configuration;
		}

		public ILogger Logger { get; }

		public IHostingEnvironment Environment { get; }

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			var idxServerOptions = Configuration.GetSection("IdxServer").Get<IdxServerOptions>();

			services.AddCors(options =>
			{
				options.AddPolicy("default", policy =>
				{
					policy.WithOrigins(idxServerOptions.ClientUrls)
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			var modules = services.AddModules(Configuration, Logger);
			var assemblies = modules.Select(x => x.GetType().Assembly).ToArray();

			var mvc = services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddRazorPagesOptions(options =>
				{
					options.AllowAreas = true;
					options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				});

			foreach (var assembly in assemblies)
			{
				mvc.AddApplicationPart(assembly);
			}

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Identity/Account/Login";
				options.LogoutPath = "/Account/Logout";
				options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			});

			var builder = services.AddIdentityServer(options =>
				{
					options.PublicOrigin = idxServerOptions.PublicOrigin;
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
				.AddInMemoryClients(Config.GetClients(idxServerOptions.ClientUrls))

				.AddAspNetIdentity<DbUser>();

			if (Environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential(); // tempkey.rsa
			}
			else
			{
				// todo: use one certificate for all instances
				builder.AddSigningCredential(new Microsoft.IdentityModel.Tokens.SigningCredentials(
					new RsaSecurityKey(RSA.Create(2048)), SecurityAlgorithms.RsaSha256Signature
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
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
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

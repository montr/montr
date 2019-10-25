using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Montr.Idx;
using Montr.Idx.Impl;
using Montr.Idx.Impl.Entities;
using Montr.Modularity;
using Montr.Web;

namespace Idx
{
	public class Startup
	{
		private ICollection<IModule> _modules;

		public Startup(ILoggerFactory loggerFactory, IWebHostEnvironment environment, IConfiguration configuration)
		{
			Logger = loggerFactory.CreateLogger<Startup>();

			Environment = environment;
			Configuration = configuration;
		}

		public ILogger Logger { get; }

		public IWebHostEnvironment Environment { get; }

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

			_modules = services.AddModules(Configuration, Logger);
			var assemblies = _modules.Select(x => x.GetType().Assembly).ToArray();

			services.AddMvc();

			var mvc = services
				.AddControllers()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddRazorPagesOptions(options =>
				{
					// options.AllowAreas = true;
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

			/*var builder = services.AddIdentityServer(options =>
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
			}*/

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

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
			// app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthorization();

			// app.UseCors("default"); // not needed, since UseIdentityServer adds cors
			// app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
			app.UseIdentityServer();

			foreach (var module in _modules.OfType<IWebModule>())
			{
				module.Configure(app);
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
				// endpoints.MapHub<MyChatHub>()
				// endpoints.MapGrpcService<MyCalculatorService>()
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}

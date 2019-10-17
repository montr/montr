using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Modularity;
using Montr.Web;
using Montr.Web.Services;

namespace Host
{
	public class Startup
	{
		private ICollection<IModule> _modules;

		public Startup(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			Logger = loggerFactory.CreateLogger<Startup>();

			Configuration = configuration;
		}

		public ILogger Logger { get; }

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddCors(options =>
			{
				options.AddPolicy("default", policy =>
				{
					policy
						.WithOrigins(
							System.Environment.GetEnvironmentVariable("PUBLIC_APP_URL"),
							System.Environment.GetEnvironmentVariable("PRIVATE_APP_URL"))
						.WithExposedHeaders("content-disposition") // to export work (fetcher.openFile) 
						.AllowCredentials()
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			_modules = services.AddModules(Configuration, Logger);
			var assemblies = _modules.Select(x => x.GetType().Assembly).ToArray();

			var mvc = services.AddMvc();

			services
				.AddControllers(options =>
				{
					options.EnableEndpointRouting = false; // todo: remove legacy routing support
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.IgnoreNullValues = true;
					options.JsonSerializerOptions.WriteIndented = false;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});
				
			foreach (var assembly in assemblies)
			{
				mvc.AddApplicationPart(assembly);
			}

			Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

			services.AddOpenIdApiAuthentication(
				Configuration.GetSection("OpenId").Get<OpenIdOptions>());

			services.AddMediatR(assemblies);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") == false, x =>
			{
				x.UseExceptionHandler("/Home/Error");
			});

			app.UseCors("default");
			app.UseHsts();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			foreach (var module in _modules.OfType<IWebModule>())
			{
				module.Configure(app);
			}

			app.UseRouting();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "greedy",
					template: "{**greedy}",
					defaults: new { controller = "Home", action = "Index" });
			});
		}
	}
}

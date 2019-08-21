using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Modularity;
using Montr.Web.Services;
using Newtonsoft.Json.Serialization;

namespace Host
{
	public class Startup
	{
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
					policy.WithOrigins(
						System.Environment.GetEnvironmentVariable("PUBLIC_APP_URL"),
						System.Environment.GetEnvironmentVariable("PRIVATE_APP_URL"))
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			var modules = services.AddModules(Configuration, Logger);
			var assemblies = modules.Select(x => x.GetType().Assembly).ToArray();

			var mvc = services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(new DefaultNamingStrategy()));
					options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
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

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

			app.UseMvc(routes =>
			{
				/* routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}"); */

				routes.MapRoute(
					name: "greedy",
					template: "{**greedy}",
					defaults: new { controller = "Home", action = "Index" });
			});
		}
	}
}

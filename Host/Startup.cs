using System.Linq;
using Kompany.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Data.Linq2Db;
using Montr.Metadata.Controllers;
using Montr.Modularity;
using Montr.Web.Controllers;
using Montr.Web.Services;
using Newtonsoft.Json.Serialization;
using Tendr.Web.Controllers;

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
			services.AddLinq2Db(
				Configuration.GetSection("ConnectionString").Get<ConnectionStringSettings>());

			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddCors(options =>
			{
				options.AddPolicy("default", policy =>
				{
					policy.WithOrigins(
							"http://tendr.montr.io:5000",
							"http://app.tendr.montr.io:5000")
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddApplicationPart(typeof(ContentController).Assembly)
				.AddApplicationPart(typeof(MetadataController).Assembly)
				.AddApplicationPart(typeof(CompanyController).Assembly)
				.AddApplicationPart(typeof(EventsController).Assembly)
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(new DefaultNamingStrategy()));
					options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				});

			services.AddOpenIdApiAuthentication(
				Configuration.GetSection("OpenId").Get<OpenIdOptions>());

			var modules = services.AddModules(Logger);

			services.AddMediatR(modules.Select(x => x.GetType().Assembly).ToArray());
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseWhen(
				context => context.Request.Path.StartsWithSegments("/api") == false,
				a =>
				{
					a.UseExceptionHandler("/Home/Error");
					
				}
			);
			app.UseHsts();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseCors("default");
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

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
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx;

namespace Host
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
				options.AddPolicy(AppConstants.CorsPolicyName, policy =>
				{
					policy
						.WithOrigins(idxServerOptions.ClientUrls)
						/*.WithOrigins(
							System.Environment.GetEnvironmentVariable("APP_URL"))*/
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
				})
				.AddRazorPagesOptions(options =>
				{
					// options.AllowAreas = true;
					// options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					// options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				});

			foreach (var assembly in assemblies)
			{
				mvc.AddApplicationPart(assembly);
			}

			Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

			services.AddMediatR(assemblies);
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") == false, x =>
			{
				x.UseExceptionHandler("/Home/Error");
			});

			app.UseHsts();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

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

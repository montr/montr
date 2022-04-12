using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
			_modules = services.AddModules(Configuration, Logger);

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = _ => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			// todo: move to idx?
			var appOptions = Configuration.GetOptions<AppOptions>();

			services.AddCors(options =>
			{
				options.AddPolicy(AppConstants.CorsPolicyName, policy =>
				{
					policy
						.WithOrigins(appOptions.ClientUrls)
						.WithExposedHeaders("content-disposition") // to export work (fetcher.openFile)
						.AllowCredentials()
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			var assemblies = _modules.Select(x => x.GetType().Assembly).ToArray();

			var mvcBuilder = services.AddMvc();

			services
				.AddControllers(_ =>
				{
				})
				.AddRazorPagesOptions(_ =>
				{
					// options.AllowAreas = true;
					// options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					// options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				});

			AddJsonOptions(mvcBuilder);

			foreach (var assembly in assemblies)
			{
				mvcBuilder.AddApplicationPart(assembly);
			}

			Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

			services.AddMediatR(assemblies);
		}

		private static void AddJsonOptions(IMvcBuilder mvcBuilder)
		{
			if (Module.UseSystemJson)
			{
				mvcBuilder.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
					options.JsonSerializerOptions.WriteIndented = false;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					options.JsonSerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<FieldMetadata>());
					// options.JsonSerializerOptions.Converters.Add(new DataFieldJsonConverter());
				});
			}
			else
			{
				mvcBuilder.AddNewtonsoftJson(options =>
				{
					// options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore; // do not use - zeros in numbers ignored also
					options.SerializerSettings.Converters.Add(new StringEnumConverter());
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				});
			}
		}

		// ReSharper disable once UnusedMember.Global
		public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
		{
			async void RunStartupTasks()
			{
				await app.ApplicationServices.RunStartupTasks(Logger);
			}

			appLifetime.ApplicationStarted.Register(RunStartupTasks);

			app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") == false, context =>
			{
				// context.SetIdentityServerOrigin(appOptions.AppUrl);
				context.UseExceptionHandler("/Home/Error");
			});

			app.UseHsts();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseRouting();

			/*foreach (var module in _modules.OfType<IWebModule>())
			{
				module.Configure(app);
			}*/

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToController("Index", "Home");
				// endpoints.MapFallbackToFile("Home/Index.cshtml");
				// endpoints.MapHub<MyChatHub>()
				// endpoints.MapGrpcService<MyCalculatorService>()
				endpoints.MapDefaultControllerRoute();
			});

			ChangeToken.OnChange(() => Configuration.GetReloadToken(),
				_ => Logger.LogInformation("Configuration changed."), Environment);
		}
	}
}

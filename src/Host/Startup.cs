using System;
using System.Collections.Concurrent;
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
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Host
{
	public class Startup
	{
		private ICollection<IModule> _modules;
		private IDictionary<string, Type> _classifierTypeMap;
		private IDictionary<string, Type> _fieldTypeMap;
		private IDictionary<string, Type> _automateConditionTypeMap;
		private IDictionary<string, Type> _automateActionTypeMap;

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

			_modules = services.AddModules(Configuration, Logger);
			var assemblies = _modules.Select(x => x.GetType().Assembly).ToArray();

			var mvcBuilder = services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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

		private void AddJsonOptions(IMvcBuilder mvcBuilder)
		{
			if (Module.UseSystemJson)
			{
				mvcBuilder.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.IgnoreNullValues = true;
					options.JsonSerializerOptions.WriteIndented = false;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					options.JsonSerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<FieldMetadata>());
					// options.JsonSerializerOptions.Converters.Add(new DataFieldJsonConverter());
				});
			}
			else
			{
				// todo: use event (?)
				_fieldTypeMap = new ConcurrentDictionary<string, Type>();
				_automateConditionTypeMap = new ConcurrentDictionary<string, Type>();
				_automateActionTypeMap = new ConcurrentDictionary<string, Type>();
				_classifierTypeMap = new ConcurrentDictionary<string, Type>();

				mvcBuilder.AddNewtonsoftJson(options =>
				{
					// options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore; // do not use - zeros in numbers ignored also

					options.SerializerSettings.Converters.Add(new StringEnumConverter());
					options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverter<FieldMetadata>(x => x.Type, _fieldTypeMap));
					options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverter<AutomationCondition>(x => x.Type, _automateConditionTypeMap));
					options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverter<AutomationAction>(x => x.Type, _automateActionTypeMap));
					options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverterWithPopulate<Classifier>(x => x.Type, _classifierTypeMap));
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				});
			}
		}

		public void Configure(IApplicationBuilder app)
		{
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

			foreach (var module in _modules.OfType<IWebModule>())
			{
				module.Configure(app);
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
				endpoints.MapFallbackToController("Index", "Home");
				// endpoints.MapHub<MyChatHub>()
				// endpoints.MapGrpcService<MyCalculatorService>()
				endpoints.MapDefaultControllerRoute();
			});

			// todo: try to remove hack to fill field type map
			var fieldProviderRegistry = app.ApplicationServices.GetRequiredService<IFieldProviderRegistry>();
			foreach (var fieldType in fieldProviderRegistry.GetFieldTypes())
			{
				_fieldTypeMap[fieldType.Code] = fieldProviderRegistry.GetFieldTypeProvider(fieldType.Code).FieldType;
			}

			var conditionProviderFactory = app.ApplicationServices.GetRequiredService<INamedServiceFactory<IAutomationConditionProvider>>();
			foreach (var name in conditionProviderFactory.GetNames())
			{
				_automateConditionTypeMap[name] = conditionProviderFactory.GetRequiredService(name).RuleType.Type;
			}

			var actionProviderFactory = app.ApplicationServices.GetRequiredService<INamedServiceFactory<IAutomationActionProvider>>();
			foreach (var name in actionProviderFactory.GetNames())
			{
				_automateActionTypeMap[name] = actionProviderFactory.GetRequiredService(name).RuleType.Type;
			}

			var classifierTypeFactory = app.ApplicationServices.GetRequiredService<INamedServiceFactory<IClassifierTypeProvider>>();
			foreach (var name in classifierTypeFactory.GetNames())
			{
				_classifierTypeMap[name] = classifierTypeFactory.GetRequiredService(name).ClassifierType;
			}
		}
	}
}

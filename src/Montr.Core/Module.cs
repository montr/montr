using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Montr.Core
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public static readonly bool UseSystemJson = false;

		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = _ => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			appBuilder.Services.AddHttpContextAccessor();

			appBuilder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			// ReSharper disable once RedundantTypeArgumentsOfMethod
			appBuilder.Services.AddScoped<IUrlHelper>(x =>
			{
				var actionContextAccessor = x.GetRequiredService<IActionContextAccessor>();
				var urlHelperFactory = x.GetRequiredService<IUrlHelperFactory>();

				return urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
			});

			appBuilder.Services.BindOptions<AppOptions>(appBuilder.Configuration);

			// todo: move to idx?
			var appOptions = appBuilder.Configuration.GetOptions<AppOptions>();

			appBuilder.Services.AddCors(options =>
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

			var assemblies = appBuilder.Modules.Select(x => x.GetType().Assembly).ToArray();

			var mvcBuilder = appBuilder.Services.AddMvc();

			appBuilder.Services
				.AddControllers(_ =>
				{
				})
				.AddRazorPagesOptions(_ =>
				{
					// options.AllowAreas = true;
					// options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					// options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				})
				/*.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory = context =>
					{
						return null;
					};
				})*/;

			AddJsonOptions(mvcBuilder);

			foreach (var assembly in assemblies)
			{
				mvcBuilder.AddApplicationPart(assembly);
			}

			// Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

			appBuilder.Services.AddMediatR(/*config => config.AsScoped(),*/ assemblies);

			appBuilder.Services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new[] { "en", "ru" };

				var defaultCulture = supportedCultures[0];

				options.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);

				options.SupportedCultures = options.SupportedUICultures = supportedCultures.Select(x => new CultureInfo(x)).ToList();

				options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
				{
					return await Task.Run(() => new ProviderCultureResult(context.Request.Cookies["lang"]));
				}));
			});

			if (UseSystemJson)
			{
				appBuilder.Services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
			}
			else
			{
				appBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
			}

			appBuilder.Services.AddSingleton<IAppUrlBuilder, DefaultAppUrlBuilder>();
			appBuilder.Services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
			appBuilder.Services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();

			appBuilder.Services.AddTransient<ICache, CombinedCache>();
			appBuilder.Services.AddTransient<ILocalizer, DefaultLocalizer>();

			appBuilder.Configuration.SetLinq2DbDefaultSettings();

			appBuilder.Services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();

			appBuilder.Services.AddTransient<IStartupTask, ImportDefaultLocaleStringListStartupTask>();

			appBuilder.Services.AddSingleton<IConfigurationRegistry, DefaultConfigurationRegistry>();
			appBuilder.Services.AddSingleton<IContentService, DefaultContentService>();
			appBuilder.Services.AddSingleton<IContentProvider, DefaultContentProvider>();
			appBuilder.Services.AddTransient<IPermissionProvider, PermissionProvider>();

			appBuilder.Services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
			appBuilder.Services.AddSingleton<EmbeddedResourceProvider, EmbeddedResourceProvider>();
			appBuilder.Services.AddSingleton<LocaleStringSerializer, LocaleStringSerializer>();

			appBuilder.Services.AddSingleton<ILocaleStringImporter, DbLocaleStringImporter>();
			appBuilder.Services.AddSingleton<IRepository<LocaleString>, DbLocaleStringRepository>();
			appBuilder.Services.AddSingleton<IAuditLogService, DbAuditLogService>();

			appBuilder.Services.AddSingleton<IRepository<EntityStatus>, DbEntityStatusRepository>();
			appBuilder.Services.AddSingleton<IEntityStatusProvider, DefaultEntityStatusProvider>();

			appBuilder.Services.AddSingleton<IPermissionResolver, DefaultPermissionResolver>();
			appBuilder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

			appBuilder.Services.AddTransient<IConfigurationProvider, DefaultConfigurationProvider>();
			appBuilder.Services.AddTransient<IRecipeExecutor, DefaultRecipeExecutor>();
			appBuilder.Services.AddTransient<IEntityRelationService, DbEntityRelationService>();

			appBuilder.Services.AddNamedTransient<IEntityProvider, ApplicationEntityProvider>(Application.EntityTypeCode);
		}

		private static void AddJsonOptions(IMvcBuilder mvcBuilder)
		{
			if (UseSystemJson)
			{
				mvcBuilder.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.MaxDepth = 64;

					options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
					options.JsonSerializerOptions.WriteIndented = false;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					// options.JsonSerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<FieldMetadata>()); // todo: restore
					// options.JsonSerializerOptions.Converters.Add(new DataFieldJsonConverter());
				});
			}
			else
			{
				mvcBuilder.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.MaxDepth = 64;

					// options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore; // do not use - zeros in numbers ignored also
					options.SerializerSettings.Converters.Add(new StringEnumConverter());
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				});
			}
		}

		public void Configure(IApp app)
		{
			async void RunStartupTasks()
			{
				await app.RunStartupTasks();
			}

			app.Lifetime.ApplicationStarted.Register(RunStartupTasks);

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

			app.UseRequestLocalization(
				app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

			/*app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToController("Index", "Home");
				// endpoints.MapFallbackToFile("Home/Index.cshtml");
				// endpoints.MapHub<MyChatHub>()
				// endpoints.MapGrpcService<MyCalculatorService>()
				endpoints.MapDefaultControllerRoute();
			});*/

			ChangeToken.OnChange(() => app.Configuration.GetReloadToken(),
				_ => app.Logger.LogInformation("Configuration changed."), app.Environment);
		}
	}
}

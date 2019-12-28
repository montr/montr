using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core.Models;
using Montr.Core.Services;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Montr.Core
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public static readonly bool UseSystemJson = false;

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddHttpContextAccessor();

			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			services.AddScoped<IUrlHelper>(x => {
				var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
				var factory = x.GetRequiredService<IUrlHelperFactory>();

				return factory.GetUrlHelper(actionContext);
			});

			var mvcBuilder = services.AddMvcCore();

			if (UseSystemJson)
			{
				mvcBuilder.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<DataField>());
					// options.JsonSerializerOptions.Converters.Add(new DataFieldJsonConverter());
				});
			}
			else
			{
				mvcBuilder.AddNewtonsoftJson(options =>
				{
					// options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore; // zeros in numbers ignored also
					options.SerializerSettings.Converters.Add(new StringEnumConverter());
					options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverter<DataField>("type", DataFieldTypes.Map));
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				});
			}

			services.Configure<AppOptions>(configuration.GetSection(typeof(AppOptions).FullName));

			services.Configure<RequestLocalizationOptions>(options =>
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
				services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
			}
			else
			{
				services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
			}

			services.AddSingleton<IAppUrlBuilder, DefaultAppUrlBuilder>();
			services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
			services.AddSingleton<IBinarySerializer, DefaultBinarySerializer>();
			services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			
			services.AddTransient<ICache, CombinedCache>();
			services.AddTransient<ILocalizer, DefaultLocalizer>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRequestLocalization(
				app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
		}
	}
}

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

namespace Montr.Core
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddHttpContextAccessor();

			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			services.AddScoped<IUrlHelper>(x => {
				var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
				var factory = x.GetRequiredService<IUrlHelperFactory>();

				return factory.GetUrlHelper(actionContext);
			});

			services
				.AddMvcCore()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<FormField>());
					// options.JsonSerializerOptions.Converters.Add(new FormFieldJsonConverter());
				});

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

			services.AddSingleton<IAppUrlBuilder, DefaultAppUrlBuilder>();
			services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
			services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
			services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();

			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
			services.AddSingleton<IContentProvider, DefaultContentProvider>();

			services.AddTransient<ILocalizer, DefaultLocalizer>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRequestLocalization(
				app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
		}
	}
}

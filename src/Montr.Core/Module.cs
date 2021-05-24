using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core.Services;

namespace Montr.Core
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule, IStartupTask
	{
		public static readonly bool UseSystemJson = false;

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddHttpContextAccessor();

			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			// ReSharper disable once RedundantTypeArgumentsOfMethod
			services.AddScoped<IUrlHelper>(x =>
			{
				var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
				var factory = x.GetRequiredService<IUrlHelperFactory>();

				return factory.GetUrlHelper(actionContext);
			});

			services.BindOptions<AppOptions>(configuration);

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
			services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();

			services.AddTransient<ICache, CombinedCache>();
			services.AddTransient<ILocalizer, DefaultLocalizer>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRequestLocalization(
				app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
		}

		public Task Run(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

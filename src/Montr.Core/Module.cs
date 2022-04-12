using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core.Services;

namespace Montr.Core
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator, IWebApplicationConfigurator, IStartupTask
	{
		public static readonly bool UseSystemJson = false;

		public void Configure(WebApplicationBuilder appBuilder)
		{
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
		}

		public void Configure(WebApplication app)
		{
			app.UseRequestLocalization(
				app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
		}

		public Task Run(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

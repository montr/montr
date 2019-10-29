using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;

namespace Montr.Core
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
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
					options.JsonSerializerOptions.Converters.Add(new FormFieldJsonConverter());
				});

			services.AddSingleton<IAppUrlBuilder, DefaultAppUrlBuilder>();
			services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
			services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
			services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();

			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
			services.AddSingleton<IContentProvider, DefaultContentProvider>();
		}
	}
}

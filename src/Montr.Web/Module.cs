using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;
using Montr.Web.Services;

namespace Montr.Web
{
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

			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
		}
	}
}

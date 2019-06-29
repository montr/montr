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

			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
		}
	}
}

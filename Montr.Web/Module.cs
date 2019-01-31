using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;
using Montr.Web.Services;

namespace Montr.Web
{
	public class Module : IModule
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpContextAccessor();

			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
		}
	}
}

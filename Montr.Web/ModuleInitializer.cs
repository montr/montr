using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;
using Montr.Web.Services;

namespace Montr.Web
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
		}
	}
}

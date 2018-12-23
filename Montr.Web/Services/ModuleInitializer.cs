using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Montr.Web.Services
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IContentProvider, DefaultContentProvider>();
		}
	}
}

using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Montr.Metadata.Services
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}
	}
}

using Microsoft.Extensions.DependencyInjection;
using Montr.Metadata.Services;
using Montr.Modularity;

namespace Montr.Metadata
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}
	}
}

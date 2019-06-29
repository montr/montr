using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Metadata.Services;
using Montr.Modularity;

namespace Montr.Metadata
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}
	}
}

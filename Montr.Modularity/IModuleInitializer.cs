using Microsoft.Extensions.DependencyInjection;

namespace Montr.Modularity
{
	public interface IModuleInitializer
	{
		void ConfigureServices(IServiceCollection services);
	}
}

using Microsoft.Extensions.DependencyInjection;

namespace Montr.Modularity
{
	public interface IModule
	{
		void ConfigureServices(IServiceCollection services);
	}
}

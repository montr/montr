using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Modularity
{
	public interface IModule
	{
		void ConfigureServices(IConfiguration configuration, IServiceCollection services);
	}
}

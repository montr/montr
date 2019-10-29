using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public interface IModule
	{
		void ConfigureServices(IConfiguration configuration, IServiceCollection services);
	}
}

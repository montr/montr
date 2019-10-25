using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Services;
using Montr.Modularity;

namespace Montr.Kompany.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<ICurrentCompanyProvider, DefaultCurrentCompanyProvider>();
		}
	}
}

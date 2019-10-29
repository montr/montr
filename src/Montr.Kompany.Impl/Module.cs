using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Services;

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

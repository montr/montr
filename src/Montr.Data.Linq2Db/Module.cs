using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Montr.Data.Linq2Db
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			configuration.SetLinq2DbDefaultSettings();

			services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}

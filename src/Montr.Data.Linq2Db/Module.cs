using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;

namespace Montr.Data.Linq2Db
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			configuration.SetLinq2DbDefaultSettings();

			services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}

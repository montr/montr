using Microsoft.Extensions.DependencyInjection;
using Montr.Core;

namespace Montr.Data.Linq2Db
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Configuration.SetLinq2DbDefaultSettings();

			appBuilder.Services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}

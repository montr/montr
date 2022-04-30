using Microsoft.Extensions.DependencyInjection;
using Montr.Core;

namespace Montr.Data.Linq2Db
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Configuration.SetLinq2DbDefaultSettings();

			appBuilder.Services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}

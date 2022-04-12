using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;

namespace Montr.Data.Linq2Db
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator
	{
		public void Configure(WebApplicationBuilder appBuilder)
		{
			appBuilder.Configuration.SetLinq2DbDefaultSettings();

			appBuilder.Services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}

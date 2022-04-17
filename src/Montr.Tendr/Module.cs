using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Tendr.Services;

namespace Montr.Tendr
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterMessageTemplateStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
		}
	}
}

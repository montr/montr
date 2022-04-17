using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Services;

namespace Montr.Idx
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, RegisterMessageTemplateStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
		}
	}
}

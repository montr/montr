using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Automate
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, RegisterMetadataStartupTask>();
		}
	}
}

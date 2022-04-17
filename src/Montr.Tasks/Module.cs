using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Tasks.Services;

namespace Montr.Tasks
{
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
		}
	}
}

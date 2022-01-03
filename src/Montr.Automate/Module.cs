using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Automate
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IAutomationContextProvider, DefaultAutomationContextProvider>();

			services.AddTransient<IStartupTask, RegisterMetadataStartupTask>();
		}
	}
}

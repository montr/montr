using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Impl.Services;
using Montr.Automate.Services;
using Montr.Core;

namespace Montr.Automate.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IAutomationService, DefaultAutomationService>();
		}
	}
}

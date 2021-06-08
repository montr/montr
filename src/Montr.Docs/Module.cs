using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Docs.Services;

namespace Montr.Docs
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}
	}
}

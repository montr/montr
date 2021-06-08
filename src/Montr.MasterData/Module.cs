using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;

namespace Montr.MasterData
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, RegisterClassifierMetadataStartupTask>();
			services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}
	}
}

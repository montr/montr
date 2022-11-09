using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Settings.Services;
using Montr.Settings.Services.Implementations;

namespace Montr.Settings
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<ISettingsRepository, DbSettingsRepository>();
			appBuilder.Services.AddTransient<ISettingsMetadataProvider, DefaultSettingsMetadataProvider>();
		}
	}
}

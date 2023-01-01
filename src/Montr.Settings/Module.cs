using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Settings.Services;
using Montr.Settings.Services.Implementations;

namespace Montr.Settings
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
			appBuilder.Services.AddSingleton<IPostStartupTask, SettingsJsonOptionsInitializer>();

			appBuilder.Services.AddSingleton<ISettingsTypeRegistry, DefaultSettingsTypeRegistry>();
			appBuilder.Services.AddTransient<ISettingsRepository, DbSettingsRepository>();

			appBuilder.Services.AddSingleton<JsonTypeProvider<ISettingsType>>();
			appBuilder.Services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, SettingsJsonOptionsConfigurator>();
		}
	}
}

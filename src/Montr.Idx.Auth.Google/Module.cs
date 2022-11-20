using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Models;
using Montr.Settings.Services;

namespace Montr.Idx.Auth.Google
{
	[Module(DependsOn = new[] { typeof(Idx.Module) })]
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<GoogleAuthOptions>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

			var authOptions = appBuilder.Configuration.GetOptions<GoogleAuthOptions>();

			if (authOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", GoogleDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = authOptions.ClientId;
					options.ClientSecret = authOptions.ClientSecret;
				});
			}
		}
	}

	public class GoogleAuthOptions : OAuthOptions
	{
	}

	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly IConfigurationRegistry _registry;

		public ConfigurationStartupTask(ISettingsTypeRegistry settingsTypeRegistry, IConfigurationRegistry registry)
		{
			_settingsTypeRegistry = settingsTypeRegistry;
			_registry = registry;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_settingsTypeRegistry.Register(typeof(GoogleAuthOptions));

			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(GoogleAuthOptions);
					settings.Category = SettingsCategory.OAuthProviders;
				});
			});

			return Task.CompletedTask;
		}
	}
}

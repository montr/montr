using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Models;
using Montr.Settings.Services;

namespace Montr.Idx.Auth.MicrosoftAccount.Services
{
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
			_settingsTypeRegistry.Register(typeof(MicrosoftAuthOptions));

			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(MicrosoftAuthOptions);
					settings.Category = SettingsCategory.OAuthProviders;
				});
			});

			return Task.CompletedTask;
		}
	}
}

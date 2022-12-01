using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Settings.Models;
using Montr.Settings.Services;

namespace Montr.Idx.Services.Implementations
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
			_settingsTypeRegistry.Register(typeof(SignInSettings));
			_settingsTypeRegistry.Register(typeof(PasswordSettings));
			_settingsTypeRegistry.Register(typeof(LockoutSettings));

			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(SignInSettings);
					settings.Category = SettingsCategory.Identity;
				});

				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(PasswordSettings);
					settings.Category = SettingsCategory.Identity;
				});

				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(LockoutSettings);
					settings.Category = SettingsCategory.Identity;
				});
			});

			_registry.Configure<Classifier>(config =>
			{
				config.When(classifier => classifier.Type == ClassifierTypeCode.Role)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "permissions";
						x.Name = "Permissions";
						x.DisplayOrder = 15;
						x.Component = ComponentCode.TabEditRolePermissions;
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.User)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "roles";
						x.Name = "Roles";
						x.DisplayOrder = 15;
						x.Icon = "solution";
						x.Component = ComponentCode.TabEditUserRoles;
					});
			});

			return Task.CompletedTask;
		}
	}
}

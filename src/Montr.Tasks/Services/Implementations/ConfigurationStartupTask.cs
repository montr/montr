using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Settings.Models;
using Montr.Settings.Services;
using Montr.Tasks.Models;

namespace Montr.Tasks.Services.Implementations
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
			_settingsTypeRegistry.Register(typeof(TasksOptions));

			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(TasksOptions);
					settings.Category = SettingsCategory.Tasks;
				});
			});

			_registry.Configure<TaskModel>(config =>
			{
				config
					//.When(task => task.StatusCode == TaskStatusCode.Open)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Information";
						x.Icon = "profile";
						x.Component = ComponentCode.PaneViewTaskInfo;
						x.Props = new { mode = "view" };
					});

				/*config
					.When(task => task.StatusCode != TaskStatusCode.Open)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Information";
						x.Icon = "profile";
						x.Component = ComponentCode.PaneViewTaskInfo;
						x.Props = new { mode = "view" };
					});*/

				config
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "History";
						x.Icon = "eye";
					})
					.Add<DataPanel>((_, x) =>
					{
						x.Key = "related-entities";
						x.Name = "Related Entities";
						x.Component = Core.ComponentCode.PanelViewRelatedEntities;
					})
					.Add<ButtonEdit>((_, x) =>
					{
						x.Key = "edit";
					});
			});

			return Task.CompletedTask;
		}
	}
}

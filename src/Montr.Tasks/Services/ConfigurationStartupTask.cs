﻿using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Services
{
	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly IConfigurationManager _configurationManager;

		public ConfigurationStartupTask(IConfigurationManager configurationManager)
		{
			_configurationManager = configurationManager;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_configurationManager.Configure<TaskModel>(config =>
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

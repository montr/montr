﻿using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services.Implementations
{
	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly IConfigurationRegistry _registry;

		public ConfigurationStartupTask(IConfigurationRegistry registry)
		{
			_registry = registry;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_registry.Configure<Classifier>(config =>
			{
				config.When(classifier => classifier.Type == ClassifierTypeCode.Automation)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "automation";
						x.Name = "Automation";
						x.DisplayOrder = 15;
						x.Component = ComponentCode.PaneEditAutomation;
					});
			});

			return Task.CompletedTask;
		}
	}
}

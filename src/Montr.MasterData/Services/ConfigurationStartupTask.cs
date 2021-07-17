using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services
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
			_configurationManager.Configure<ClassifierType>(config =>
			{
				config
					.Add(new DataPane { Key = "info", Name = "Информация", Icon = "profile", Component = "panes/TabEditClassifierType" })
					.Add(new DataPane { Key = "hierarchy", Name = "Иерархия", Component = "panes/TabEditClassifierTypeHierarchy" })
					.Add(new DataPane { Key = "fields", Name = "Поля", Component = "panes/PaneSearchMetadata" })
					.Add(new DataPane { Key = "numeration", Name = "Нумерация", Component = "panes/PaneEditNumeration" })
					.Add(new DataPane { Key = "history", Name = "History", Icon = "eye" });
			});

			return Task.CompletedTask;
		}
	}
}

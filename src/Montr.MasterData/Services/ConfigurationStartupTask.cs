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

			_configurationManager.Configure<Classifier>(config =>
			{
				config
					.Add(new DataPane { Key = "info", Name = "Информация", DisplayOrder = 10, Icon = "profile", Component = "panes/TabEditClassifier" })
					.Add(new DataPane { Key = "hierarchy", Name = "Иерархия", DisplayOrder = 20, Component = "panes/TabEditClassifierHierarchy" })
					.Add(new DataPane { Key = "dependencies", Name = "Зависимости", DisplayOrder = 30 })
					.Add(new DataPane { Key = "history", Name = "История изменений", DisplayOrder = 40, Icon = "eye" });

				config.When(classifier => classifier.Type == ClassifierTypeCode.Numerator)
					.Add(new DataPane { Key = "usage", Name = "Использование", DisplayOrder = 15, Component = "panes/TabEditNumeratorEntities" });
			});

			return Task.CompletedTask;
		}
	}
}

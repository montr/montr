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
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Информация";
						x.Icon = "profile";
						x.Component = "panes/TabEditClassifierType";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "hierarchy";
						x.Name = "Иерархия";
						x.Component = "panes/TabEditClassifierTypeHierarchy";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "fields";
						x.Name = "Поля";
						x.Component = "panes/PaneSearchMetadata";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "numeration";
						x.Name = "Нумерация";
						x.Component = "panes/PaneEditNumeration";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "History";
						x.Icon = "eye";
					});
			});

			_configurationManager.Configure<Classifier>(config =>
			{
				config
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Информация";
						x.DisplayOrder = 10;
						x.Icon = "profile";
						x.Component = "panes/TabEditClassifier";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "hierarchy";
						x.Name = "Иерархия";
						x.DisplayOrder = 20;
						x.Component = "panes/TabEditClassifierHierarchy";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "dependencies";
						x.Name = "Зависимости";
						x.DisplayOrder = 30;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "История изменений";
						x.DisplayOrder = 40;
						x.Icon = "eye";
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.Numerator)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "usage";
						x.Name = "Использование";
						x.DisplayOrder = 15;
						x.Component = "panes/TabEditNumeratorEntities";
					});
			});

			return Task.CompletedTask;
		}
	}
}

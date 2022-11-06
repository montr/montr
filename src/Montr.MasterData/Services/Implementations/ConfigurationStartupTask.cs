using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services.Implementations
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
						x.Name = "Information";
						x.Icon = "profile";
						x.Component = ComponentCode.TabEditClassifierType;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "hierarchy";
						x.Name = "Hierarchy";
						x.Component = ComponentCode.TabEditClassifierTypeHierarchy;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "fields";
						x.Name = "Fields";
						x.Component = Core.ComponentCode.PaneEditFieldsMetadata;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "numeration";
						x.Name = "Numeration";
						x.Component = ComponentCode.TabEditNumeration;
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
						x.Name = "Information";
						x.DisplayOrder = 10;
						x.Icon = "profile";
						x.Component = ComponentCode.TabEditClassifier;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "hierarchy";
						x.Name = "Hierarchy";
						x.DisplayOrder = 20;
						x.Component = ComponentCode.TabEditClassifierHierarchy;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "dependencies";
						x.Name = "Dependencies";
						x.DisplayOrder = 30;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "History";
						x.DisplayOrder = 40;
						x.Icon = "eye";
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.Numerator)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "usage";
						x.Name = "Usages";
						x.DisplayOrder = 15;
						x.Component = ComponentCode.TabEditNumeratorEntities;
					});
			});

			return Task.CompletedTask;
		}
	}
}

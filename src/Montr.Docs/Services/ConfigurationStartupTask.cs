using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Services
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
			_configurationManager.Configure<Classifier>(config =>
			{
				config.When(classifier => classifier.Type == ClassifierTypeCode.Questionnaire)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "questionnaire";
						x.Name = "Вопросы";
						x.DisplayOrder = 15;
						x.Component = "panes/PaneSearchMetadata";
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.DocumentType)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "fields";
						x.Name = "Анкета";
						x.DisplayOrder = 15;
						x.Component = "panes/PaneSearchMetadata";
					})

					// todo: move to processes (?)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "statuses";
						x.Name = "Statuses";
						x.DisplayOrder = 16;
						x.Component = "panes/PaneSearchEntityStatuses";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "automation";
						x.Name = "Automations";
						x.DisplayOrder = 17;
						x.Component = "panes/PaneSearchAutomation";
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.Process)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "steps";
						x.Name = "Шаги";
						x.DisplayOrder = 15;
						x.Component = "panes/PaneProcessStepList";
					});
			});

			_configurationManager.Configure<Document>(config =>
			{
				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "form";
						x.Name = "Form";
						x.Component = ComponentCode.PaneViewDocumentForm;
						x.Props = new { mode = "edit" };
					})
					.Add<Button>((document, x) =>
					{
						x.Key = "publish";
						x.Name = "Publish";
						x.Action = "/document/publish";
						x.Props = new PublishDocument { DocumentUid = document.Uid };
					});

				config.When(document => document.StatusCode != DocumentStatusCode.Draft)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "form";
						x.Name = "Form";
						x.Component = ComponentCode.PaneViewDocumentForm;
						x.Props = new { mode = "view" };
					})
					.Add<Button>((_, x) =>
					{
						x.Name ="Accept or Reject";
					});

				// for any status
				config
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "History";
						x.Icon = "eye";
					});
			});

			return Task.CompletedTask;
		}
	}
}

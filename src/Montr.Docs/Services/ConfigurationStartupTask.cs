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
					.Add(new DataPane { Key = "questionnaire", Name = "Вопросы", DisplayOrder = 15, Component = "panes/PaneSearchMetadata" });

				config.When(classifier => classifier.Type == ClassifierTypeCode.DocumentType)
					.Add(new DataPane { Key = "fields", Name = "Анкета", DisplayOrder = 15, Component = "panes/PaneSearchMetadata" })

					// todo: move to processes (?)
					.Add(new DataPane { Key = "statuses", Name = "Statuses", DisplayOrder = 16, Component = "panes/PaneSearchEntityStatuses" })
					.Add(new DataPane { Key = "automation", Name = "Automations", DisplayOrder = 17, Component = "panes/PaneSearchAutomation" });

				config.When(classifier => classifier.Type == ClassifierTypeCode.Process)
					.Add(new DataPane { Key = "steps", Name = "Шаги", DisplayOrder = 15, Component = "panes/PaneProcessStepList" });
			});

			_configurationManager.Configure<Document>(config =>
			{
				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add(new DataPane { Key = "form", Name = "Form", Component = ComponentCode.PaneViewDocumentForm, Props = new { mode = "edit" } })
					.Add(new Button { Key = "publish", Name = "Publish", Action = "/document/publish" })
					.Add<Button>((document, x) =>
					{
						x.Key = "publish";
						x.Name = "Publish";
						x.Action = "/document/publish";
						x.Props = new PublishDocument { DocumentUid = document.Uid };
					});

				config.When(document => document.StatusCode != DocumentStatusCode.Draft)
					.Add(new DataPane { Key = "form", Name = "Form", Component = ComponentCode.PaneViewDocumentForm, Props = new { mode = "view" } })
					.Add(new Button { Name = "Accept or Reject" });

				config.Add(new DataPane { Key = "history", Name = "History", Icon = "eye" });
			});

			return Task.CompletedTask;
		}
	}
}

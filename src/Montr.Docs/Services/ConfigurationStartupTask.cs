using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
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
					.Add(new DataPane { Key = "form", Name = "Form", Component = "pane_view_document_form", Props = new { mode = "edit" } });

				config.When(document => document.StatusCode != DocumentStatusCode.Draft)
					.Add(new DataPane { Key = "form", Name = "Form", Component = "pane_view_document_form", Props = new { mode = "view" } });

				config.Add(new DataPane { Key = "history", Name = "History", Icon = "eye" });
			});

			return Task.CompletedTask;
		}
	}
}

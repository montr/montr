﻿using System.Threading;
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
						x.Key = "form";
						x.Name = "Form";
						x.DisplayOrder = 15;
						x.Component = Core.ComponentCode.PaneEditFormMetadata;
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.DocumentType)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "form";
						x.Name = "Form";
						x.DisplayOrder = 15;
						x.Component = Core.ComponentCode.PaneEditFormMetadata;
					})

					.Add<DataPane>((_, x) =>
					{
						x.Key = "numeration";
						x.Name = "Numeration";
						x.DisplayOrder = 20;
						x.Component = MasterData.ComponentCode.TabEditNumeration;
					})

					// todo: move to processes (?)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "statuses";
						x.Name = "Statuses";
						x.DisplayOrder = 25;
						x.Component = Core.ComponentCode.PaneSearchEntityStatuses;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "automation";
						x.Name = "Automations";
						x.DisplayOrder = 30;
						x.Component = Automate.ComponentCode.PaneSearchAutomation;
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.Process)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "steps";
						x.Name = "Steps";
						x.DisplayOrder = 30;
						x.Component = ComponentCode.PaneListProcessStep;
					});
			});

			_configurationManager.Configure<Document>(config =>
			{
				config //.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Information";
						x.Icon = "profile";
						x.Component = ComponentCode.PaneViewDocumentInfo;
						// x.Props = new {mode = "edit"};
					})
					.Add<DataPane>((document, x) =>
					{
						x.Key = "form";
						x.Name = "Form";
						x.Component = ComponentCode.PaneViewDocumentForm;
						x.Props = new {mode = document.StatusCode == DocumentStatusCode.Draft ? "edit" : "view"};
					});

				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					/*.Add<Button>((document, x) =>
					{
						x.Key = "submit";
						x.Name = "Submit";
						x.Type = ButtonType.Primary;
						x.Action = "/companyRegistrationRequest/submit";
						x.Props = new SubmitDocument { DocumentUid = document.Uid };
					})*/;

				config.When(document => document.StatusCode != DocumentStatusCode.Draft)
					/*.Add<Button>((_, x) =>
					{
						x.Name = "Accept or Reject";
					})*/;

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

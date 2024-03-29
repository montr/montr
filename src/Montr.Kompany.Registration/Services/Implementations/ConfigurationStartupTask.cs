﻿using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs;
using Montr.Docs.Commands;
using Montr.Docs.Models;
using Montr.Kompany.Registration.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Settings.Models;
using Montr.Settings.Services;

namespace Montr.Kompany.Registration.Services.Implementations
{
	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly IConfigurationRegistry _registry;

		public ConfigurationStartupTask(ISettingsTypeRegistry settingsTypeRegistry, IConfigurationRegistry registry)
		{
			_settingsTypeRegistry = settingsTypeRegistry;
			_registry = registry;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_settingsTypeRegistry.Register(typeof(CompanyRegistrationOptions));

			_registry.Configure<Classifier>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(CompanyRegistrationOptions);
				});
			});

			_registry.Configure<Document>(config =>
			{
				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add<Button>((document, x) =>
					{
						x.Key = "submit_1";
						x.Name = "Submit";
						x.Type = ButtonType.Primary;
						x.Action = "/companyRegistrationRequest/submit";
						x.Permission = Permission.GetCode(typeof(Docs.Permissions.SubmitDocument));
						x.Props = new SubmitCompanyRegistrationRequest { DocumentUid = document.Uid };
					});

				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add<Button>((document, x) =>
					{
						x.Key = "submit_2";
						x.Name = "Submit";
						x.Type = ButtonType.Primary;
						x.Action = "/document/changeStatus";
						// x.Permission = Permission.GetCode(typeof(Docs.Permissions.SubmitDocument));
						x.Props = new ChangeDocumentStatus { DocumentUid = document.Uid, StatusCode = DocumentStatusCode.Submitted };
					});

				config.When(document => document.StatusCode == DocumentStatusCode.Submitted)
					.Add<CreateRelatedDocument>((document, x) =>
					{
						x.Key = "accept";
						x.Name = "Accept or Reject";
						x.Type = ButtonType.Primary;
						x.Action = "/document/createRelated";
						x.Permission = Permission.GetCode(typeof(Docs.Permissions.AcceptDocument));
						x.DocumentTypeCode = DocumentTypes.CompanyRegistrationRequest;
						x.RelationTypeCode = RelationTypeCode.Approval;
						x.Props = new CreateRelatedDocument { DocumentUid = document.Uid };
					});
			});

			return Task.CompletedTask;
		}
	}
}

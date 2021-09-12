﻿using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs;
using Montr.Docs.Models;
using Montr.Kompany.Registration.Commands;
using Montr.Metadata.Models;

namespace Montr.Kompany.Registration.Services
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
			_configurationManager.Configure<Document>(config =>
			{
				config.When(document => document.StatusCode == DocumentStatusCode.Draft)
					.Add<Button>((document, x) =>
					{
						x.Key = "submit";
						x.Name = "Submit";
						x.Type = ButtonType.Primary;
						x.Action = "/companyRegistrationRequest/submit";
						x.Props = document.Uid.HasValue ? new SubmitCompanyRegistrationRequest { DocumentUid = document.Uid.Value } : null;
					});

				config.When(document => document.StatusCode != DocumentStatusCode.Draft)
					.Add<Button>((_, x) =>
					{
						x.Name = "Accept or Reject";
					});
			});

			return Task.CompletedTask;
		}
	}
}

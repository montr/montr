using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs;
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
						x.Key = "submit";
						x.Name = "Submit";
						x.Type = ButtonType.Primary;
						x.Action = "/companyRegistrationRequest/submit";
						x.Props = document.Uid.HasValue
							? new SubmitCompanyRegistrationRequest { DocumentUid = document.Uid.Value }
							: null;
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

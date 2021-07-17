using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;
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

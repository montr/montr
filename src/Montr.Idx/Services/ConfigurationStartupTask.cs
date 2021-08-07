using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Idx.Services
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
				config.When(classifier => classifier.Type == ClassifierTypeCode.Role)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "permissions";
						x.Name = "Permissions";
						x.DisplayOrder = 15;
						x.Component = "components/tab-edit-role-permissions";
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.User)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "roles";
						x.Name = "Roles";
						x.DisplayOrder = 15;
						x.Icon = "solution";
						x.Component = "components/tab-edit-user-roles";
					});
			});

			return Task.CompletedTask;
		}
	}
}

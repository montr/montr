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
					.Add(new DataPane { Key = "permissions", Name = "Permissions", DisplayOrder = 15, Component = "components/tab-edit-role-permissions" });

				config.When(classifier => classifier.Type == ClassifierTypeCode.User)
					.Add(new DataPane { Key = "roles", Name = "Roles", DisplayOrder = 15, Icon = "solution", Component = "components/tab-edit-user-roles" });
			});

			return Task.CompletedTask;
		}
	}
}

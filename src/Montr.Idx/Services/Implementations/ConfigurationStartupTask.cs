using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Idx.Services.Implementations
{
	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly IConfigurationRegistry _registry;

		public ConfigurationStartupTask(IConfigurationRegistry registry)
		{
			_registry = registry;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_registry.Configure<Classifier>(config =>
			{
				config.When(classifier => classifier.Type == ClassifierTypeCode.Role)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "permissions";
						x.Name = "Permissions";
						x.DisplayOrder = 15;
						x.Component = ComponentCode.TabEditRolePermissions;
					});

				config.When(classifier => classifier.Type == ClassifierTypeCode.User)
					.Add<DataPane>((_, x) =>
					{
						x.Key = "roles";
						x.Name = "Roles";
						x.DisplayOrder = 15;
						x.Icon = "solution";
						x.Component = ComponentCode.TabEditUserRoles;
					});
			});

			return Task.CompletedTask;
		}
	}
}

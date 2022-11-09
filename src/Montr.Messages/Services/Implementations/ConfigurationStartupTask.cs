using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Models;

namespace Montr.Messages.Services.Implementations
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
			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.OptionsType = typeof(SmtpOptions);
				});
			});

			return Task.CompletedTask;
		}
	}
}

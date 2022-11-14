using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;

namespace Montr.Settings.Services.Implementations
{
	public class SettingsJsonOptionsInitializer : IPostStartupTask
	{
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly JsonTypeProvider<ISettingsType> _settingsTypeProvider;

		public SettingsJsonOptionsInitializer(
			ISettingsTypeRegistry settingsTypeRegistry,
			JsonTypeProvider<ISettingsType> settingsTypeProvider)
		{
			_settingsTypeRegistry = settingsTypeRegistry;
			_settingsTypeProvider = settingsTypeProvider;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			foreach (var (typeCode, type) in _settingsTypeRegistry.GetRegisteredTypes())
			{
				_settingsTypeProvider.Register(typeCode, type);
			}

			return Task.CompletedTask;
		}
	}
}

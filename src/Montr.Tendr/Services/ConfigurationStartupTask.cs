using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Services
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
			_configurationManager.Configure<Event>(config =>
			{
				config
					.Add(new DataPane { Key = "info", Name = "Информация", Icon = "profile", Component = "panes/private/EditEventPane" })
					.Add(new DataPane { Key = "invitations", Name = "Приглашения (😎)", Icon = "solution", Component = "panes/private/InvitationPane" })
					.Add(new DataPane { Key = "proposals", Name = "Предложения", Icon = "solution" })
					.Add(new DataPane { Key = "questions", Name = "Разъяснения", Icon = "solution" })
					.Add(new DataPane { Key = "team", Name = "Команда", Icon = "team" })
					.Add(new DataPane { Key = "items", Name = "Позиции", Icon = "table" })
					.Add(new DataPane { Key = "history", Name = "История изменений", Icon = "eye" })
					.Add(new DataPane { Key = "5", Name = "Тендерная комиссия (команда?)" })
					.Add(new DataPane { Key = "6", Name = "Критерии оценки (анкета?)" })
					.Add(new DataPane { Key = "7", Name = "Документы (поле?)" })
					.Add(new DataPane { Key = "8", Name = "Контактные лица (поле?)" });
			});

			return Task.CompletedTask;
		}
	}
}

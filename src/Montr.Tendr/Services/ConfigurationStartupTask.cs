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
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Информация";
						x.Icon = "profile";
						x.Component = "panes/private/EditEventPane";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "invitations";
						x.Name = "Приглашения (😎)";
						x.Icon = "solution";
						x.Component = "panes/private/InvitationPane";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "proposals";
						x.Name = "Предложения";
						x.Icon = "solution";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "questions";
						x.Name = "Разъяснения";
						x.Icon = "solution";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "team";
						x.Name = "Команда";
						x.Icon = "team";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "items";
						x.Name = "Позиции";
						x.Icon = "table";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "История изменений";
						x.Icon = "eye";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "5";
						x.Name = "Тендерная комиссия (команда?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "6";
						x.Name = "Критерии оценки (анкета?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "7";
						x.Name = "Документы (поле?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "8";
						x.Name = "Контактные лица (поле?)";
					});
			});

			return Task.CompletedTask;
		}
	}
}

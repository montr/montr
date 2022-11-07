using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Services
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
			_registry.Configure<Event>(config =>
			{
				config
					.Add<DataPane>((_, x) =>
					{
						x.Key = "info";
						x.Name = "Information";
						x.Icon = "profile";
						x.Component = ComponentCode.TabEditEvent;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "invitations";
						x.Name = "Invitations (😎)";
						x.Icon = "solution";
						x.Component = ComponentCode.TabEditInvitations;
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "proposals";
						x.Name = "Proposals";
						x.Icon = "solution";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "questions";
						x.Name = "Questions";
						x.Icon = "solution";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "team";
						x.Name = "Team";
						x.Icon = "team";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "items";
						x.Name = "Items";
						x.Icon = "table";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "history";
						x.Name = "History";
						x.Icon = "eye";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "5";
						x.Name = "Commission (Team?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "6";
						x.Name = "Criteria (Form?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "7";
						x.Name = "Documents (field?)";
					})
					.Add<DataPane>((_, x) =>
					{
						x.Key = "8";
						x.Name = "Contacts (field?)";
					});
			});

			return Task.CompletedTask;
		}
	}
}

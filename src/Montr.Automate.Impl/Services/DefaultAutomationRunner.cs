using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;

namespace Montr.Automate.Impl.Services
{
	public class DefaultAutomationRunner : IAutomationRunner
	{
		private readonly IRepository<Automation> _repository;
		private readonly IAutomationProviderRegistry _providerRegistry;

		public DefaultAutomationRunner(IRepository<Automation> repository, IAutomationProviderRegistry providerRegistry)
		{
			_repository = repository;
			_providerRegistry = providerRegistry;
		}

		public async Task Run(AutomationContext context, CancellationToken cancellationToken)
		{
			var automations = await _repository.Search(new AutomationSearchRequest
			{
				EntityTypeCode = context.EntityTypeCode,
				EntityTypeUid = context.EntityTypeUid,
				IsActive = true,
				PageSize = 100
			}, cancellationToken);

			foreach (var automation in automations.Rows)
			{
				if (await MeetAll(automation.Conditions, context, cancellationToken))
				{
					await Execute(automation.Actions, context, cancellationToken);
				}
			}
		}

		private async Task<bool> MeetAll(IEnumerable<AutomationCondition> conditions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (conditions != null)
			{
				foreach (var condition in conditions)
				{
					var provider = _providerRegistry.GetConditionProvider(condition.Type);

					if (await provider.Meet(condition, context, cancellationToken) == false)
					{
						return false;
					}
				}
			}

			return true;
		}

		private async Task Execute(IEnumerable<AutomationAction> actions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (actions != null)
			{
				foreach (var action in actions)
				{
					var provider = _providerRegistry.GetActionProvider(action.Type);

					await provider.Execute(action, context, cancellationToken);
				}
			}
		}
	}
}

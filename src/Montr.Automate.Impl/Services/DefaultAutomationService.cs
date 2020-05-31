using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;

namespace Montr.Automate.Impl.Services
{
	public class DefaultAutomationService : IAutomationService
	{
		private readonly IRepository<Automation> _repository;
		private readonly IAutomationProviderRegistry _providerRegistry;

		public DefaultAutomationService(IRepository<Automation> repository, IAutomationProviderRegistry providerRegistry)
		{
			_repository = repository;
			_providerRegistry = providerRegistry;
		}

		public async Task OnChange(AutomationContext context, CancellationToken cancellationToken)
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
				if (await MeetConditions(automation, context, cancellationToken))
				{
					await ExecuteActions(automation, context, cancellationToken);
				}
			}
		}

		private async Task<bool> MeetConditions(Automation automation, AutomationContext context, CancellationToken cancellationToken)
		{
			var meetAllConditions = automation.Conditions.Where(x => x.Meet == AutomationConditionMeet.All).ToList();

			foreach (var condition in meetAllConditions)
			{
				if (await MeetCondition(condition, context, cancellationToken) == false)
				{
					return false;
				}
			}

			var meetAnyConditions = automation.Conditions.Where(x => x.Meet == AutomationConditionMeet.Any).ToList();

			if (meetAnyConditions.Count > 0)
			{
				var meetAny = false;

				foreach (var condition in meetAnyConditions)
				{
					if (await MeetCondition(condition, context, cancellationToken) == false)
					{
						meetAny = true;
						break;
					}
				}

				if (meetAny == false)
				{
					return false;
				}
			}

			return true;
		}

		private async Task<bool> MeetCondition(AutomationCondition condition, AutomationContext context, CancellationToken cancellationToken)
		{
			var provider = _providerRegistry.GetConditionProvider(condition.Type);

			return await provider.Meet(condition, context, cancellationToken);
		}

		private async Task ExecuteActions(Automation automation, AutomationContext context, CancellationToken cancellationToken)
		{
			foreach (var action in automation.Actions)
			{
				var provider = _providerRegistry.GetActionProvider(action.Type);

				await provider.Execute(action, context, cancellationToken);
			}
		}
	}
}

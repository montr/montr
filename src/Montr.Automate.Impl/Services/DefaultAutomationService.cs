using System;
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

		public DefaultAutomationService(IRepository<Automation> repository)
		{
			_repository = repository;
		}

		public async Task OnChange(string entityTypeCode, Guid entityUid, object entity, CancellationToken cancellationToken)
		{
			var automations = await _repository.Search(new AutomationSearchRequest
			{
				EntityTypeCode = entityTypeCode,
				EntityUid = entityUid,
				PageSize = 100
			}, cancellationToken);

			foreach (var automation in automations.Rows)
			{
				if (await MeetConditions(automation, entity, cancellationToken))
				{
					await ExecuteActions(automation, entity, cancellationToken);
				}
			}
		}

		private async Task<bool> MeetConditions(Automation automation, object entity, CancellationToken cancellationToken)
		{
			var meetAllConditions = automation.Conditions.Where(x => x.Meet == AutomationConditionMeet.All).ToList();

			foreach (var condition in meetAllConditions)
			{
				if (await MeetCondition(condition, entity, cancellationToken) == false)
				{
					return false;
				}
			}

			var meetAnyConditions = automation.Conditions.Where(x => x.Meet == AutomationConditionMeet.Any).ToList();

			if (meetAnyConditions.Count > 0)
			{
				bool meetAny = false;

				foreach (var condition in meetAnyConditions)
				{
					if (await MeetCondition(condition, entity, cancellationToken) == false)
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

		private async Task<bool> MeetCondition(AutomationCondition condition, object entity, CancellationToken cancellationToken)
		{
			return await Task.FromResult(true);
		}

		private async Task ExecuteActions(Automation automation, object entity, CancellationToken cancellationToken)
		{
			foreach (var action in automation.Actions)
			{
			}

			await Task.CompletedTask;
		}
	}
}

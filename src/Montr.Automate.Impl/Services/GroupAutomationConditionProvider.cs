using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;

namespace Montr.Automate.Impl.Services
{
	public class GroupAutomationConditionProvider : IAutomationConditionProvider
	{
		private readonly IAutomationProviderRegistry _providerRegistry;

		public GroupAutomationConditionProvider(IAutomationProviderRegistry providerRegistry)
		{
			_providerRegistry = providerRegistry;
		}

		public Type ConditionType => typeof(GroupAutomationCondition);

		public async Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
		{
			var group = (GroupAutomationCondition)automationCondition;

			if (group.Meet == AutomationConditionMeet.All)
			{
				return await MeetAll(group.Conditions, context, cancellationToken);
			}

			return await MeetAny(group.Conditions, context, cancellationToken);
		}

		private async Task<bool> MeetAll(IEnumerable<AutomationCondition> conditions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (conditions != null)
			{
				foreach (var condition in conditions)
				{
					if (await MeetCondition(condition, context, cancellationToken) == false)
					{
						return false;
					}
				}
			}

			// if collection empty or all meet
			return true;
		}

		private async Task<bool> MeetAny(IEnumerable<AutomationCondition> conditions, AutomationContext context, CancellationToken cancellationToken)
		{
			if (conditions != null)
			{
				foreach (var condition in conditions)
				{
					if (await MeetCondition(condition, context, cancellationToken))
					{
						return true;
					}
				}
			}

			// if collection empty or all not meet
			return false;
		}

		private async Task<bool> MeetCondition(AutomationCondition condition, AutomationContext context, CancellationToken cancellationToken)
		{
			var provider = _providerRegistry.GetConditionProvider(condition.Type);

			return await provider.Meet(condition, context, cancellationToken);
		}
	}
}

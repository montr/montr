using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services.Implementations
{
	public class GroupAutomationConditionProvider : IAutomationConditionProvider
	{
		private readonly IAutomationProviderRegistry _providerRegistry;

		public GroupAutomationConditionProvider(IAutomationProviderRegistry providerRegistry)
		{
			_providerRegistry = providerRegistry;
		}

		public AutomationRuleType RuleType => new()
		{
			Code = GroupAutomationCondition.TypeCode,
			Name = "Group",
			Type = typeof(GroupAutomationCondition)
		};

		public Task<IList<FieldMetadata>> GetMetadata(
			AutomationContext context, AutomationCondition condition, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
		{
			var condition = (GroupAutomationCondition)automationCondition;

			var props = condition.Props;

			if (props.Meet == AutomationConditionMeet.All)
			{
				return await MeetAll(props.Conditions, context, cancellationToken);
			}

			return await MeetAny(props.Conditions, context, cancellationToken);
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;

namespace Montr.Automate.Impl.Services
{
	public class FieldAutomationConditionProvider : IAutomationConditionProvider
	{
		private readonly IAutomationContextProvider _automationContextProvider;

		public FieldAutomationConditionProvider(IAutomationContextProvider automationContextProvider)
		{
			_automationContextProvider = automationContextProvider;
		}

		public AutomationRuleType RuleType => new AutomationRuleType
		{
			Code = FieldAutomationCondition.TypeCode,
			Name = "Field",
			Type = typeof(FieldAutomationCondition)
		};

		public async Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
		{
			var condition = (FieldAutomationCondition)automationCondition;

			var entity = await _automationContextProvider.GetEntity(context, cancellationToken);

			var props = condition.Props;

			var result = false;

			// todo: move to automation context provider (?)
			var property = entity.GetType().GetProperty(props.Field);

			if (property != null)
			{
				var getMethod = property.GetGetMethod();

				if (getMethod != null)
				{
					var value = Convert.ToString(getMethod.Invoke(entity, null));

					var compareResult = string.Compare(value, props.Value, StringComparison.OrdinalIgnoreCase);

					switch (props.Operator)
					{
						case AutomationConditionOperator.Equal:
							result = compareResult == 0;
							break;
						case AutomationConditionOperator.NotEqual:
							result = compareResult != 0;
							break;
						case AutomationConditionOperator.LessThan:
							result = compareResult < 0;
							break;
						case AutomationConditionOperator.LessThanEqual:
							result = compareResult <= 0;
							break;
						case AutomationConditionOperator.GreaterThan:
							result = compareResult > 0;
							break;
						case AutomationConditionOperator.GreaterThanEqual:
							result = compareResult >= 0;
							break;
						default:
							throw new InvalidOperationException($"Operator {props.Operator} is not supported.");
					}
				}
			}

			return result;
		}
	}
}

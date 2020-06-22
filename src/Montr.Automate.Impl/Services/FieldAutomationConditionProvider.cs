using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;

namespace Montr.Automate.Impl.Services
{
	public class FieldAutomationConditionProvider : IAutomationConditionProvider
	{
		public Type ConditionType => typeof(FieldAutomationCondition);

		public Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
		{
			var condition = (FieldAutomationCondition)automationCondition;

			var props = condition.Props;

			var result = false;

			var property = context.Entity.GetType().GetProperty(props.Field);

			if (property != null)
			{
				var getMethod = property.GetGetMethod();

				if (getMethod != null)
				{
					var value = Convert.ToString(getMethod.Invoke(context.Entity, null));

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

			return Task.FromResult(result);
		}
	}
}

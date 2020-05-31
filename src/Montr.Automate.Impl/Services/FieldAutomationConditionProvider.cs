using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;

namespace Montr.Automate.Impl.Services
{
	public class FieldAutomationConditionProvider : IAutomationConditionProvider
	{
		public Task<bool> Meet(AutomationCondition automationCondition, object entity, CancellationToken cancellationToken)
		{
			var result = false;

			var condition = (FieldAutomationCondition)automationCondition;

			var property = entity.GetType().GetProperty(condition.Field);

			if (property != null)
			{
				var getMethod = property.GetGetMethod();

				if (getMethod != null)
				{
					var value = Convert.ToString(getMethod.Invoke(entity, null));

					var compareResult = string.Compare(value, condition.Value, StringComparison.OrdinalIgnoreCase);

					switch (condition.Operator)
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
							throw new InvalidOperationException($"Operator {condition.Operator} is not supported.");
					}
				}
			}

			return Task.FromResult(result);
		}
	}
}

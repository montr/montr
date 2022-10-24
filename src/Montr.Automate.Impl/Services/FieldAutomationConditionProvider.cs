﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.Services
{
	public class FieldAutomationConditionProvider : IAutomationConditionProvider
	{
		private readonly IAutomationContextProvider _automationContextProvider;

		public FieldAutomationConditionProvider(IAutomationContextProvider automationContextProvider)
		{
			_automationContextProvider = automationContextProvider;
		}

		public AutomationRuleType RuleType => new()
		{
			Code = FieldAutomationCondition.TypeCode,
			Name = "Field",
			Type = typeof(FieldAutomationCondition)
		};

		public async Task<IList<FieldMetadata>> GetMetadata(AutomationContext context, CancellationToken cancellationToken)
		{
			var fields = await _automationContextProvider.GetFields(context, cancellationToken);

			var fieldOptions = fields
				.Select(x => new SelectFieldOption { Value = x.Key, Name = x.Name })
				.ToArray();

			var operators = new[]
			{
				new SelectFieldOption { Value = "Equal", Name = "=" },
				new SelectFieldOption { Value = "NotEqual", Name = "≠" },
				new SelectFieldOption { Value = "LessThan", Name = "<" },
				new SelectFieldOption { Value = "LessThanEqual", Name = "≤" },
				new SelectFieldOption { Value = "GreaterThan", Name = ">" },
				new SelectFieldOption { Value = "GreaterThanEqual", Name = "≥" }
			};

			return new FieldMetadata[]
			{
				new SelectField { Key = "field", Name = "Select field", Required = true, Props = { Options = fieldOptions } },
				new SelectField { Key = "operator", Name = "Operator", Required = true, Props = { Options = operators } },
				new TextField { Key = "value", Name = "Value", Required = true }
			};
		}

		public async Task<bool> Meet(AutomationCondition automationCondition, AutomationContext context, CancellationToken cancellationToken)
		{
			var condition = (FieldAutomationCondition)automationCondition;

			var entity = await _automationContextProvider.GetEntity(context, cancellationToken);

			var props = condition.Props;

			var result = false;

			// todo: use GetFields from automation context provider (?)
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

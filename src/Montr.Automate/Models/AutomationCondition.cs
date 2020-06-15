using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public abstract class AutomationCondition
	{
		public abstract string Type { get; }
	}

	public enum AutomationConditionMeet
	{
		All,
		Any
	}

	public enum AutomationConditionOperator
	{
		Equal,
		NotEqual,
		LessThan,
		LessThanEqual,
		GreaterThan,
		GreaterThanEqual
	}

	public class GroupAutomationCondition : AutomationCondition
	{
		public const string TypeCode = "group";

		public override string Type => TypeCode;

		public AutomationConditionMeet Meet { get; set; }

		public IList<AutomationCondition> Conditions { get; set; }
	}

	public class FieldAutomationCondition : AutomationCondition
	{
		public const string TypeCode = "field";

		public override string Type => TypeCode;

		public string Field { get; set; }

		public AutomationConditionOperator Operator { get; set; }

		public string Value { get; set; }
	}
}

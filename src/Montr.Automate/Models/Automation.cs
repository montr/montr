using System;
using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public class Automation
	{
		public Guid Uid { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool Active { get; set; }

		public IList<AutomationCondition> Conditions { get; set; }

		public IList<AutomationAction> Actions { get; set; }
	}

	public abstract class AutomationCondition
	{
		public abstract string Type { get; }

		public AutomationConditionMeet Meet { get; set; }
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
		GreaterThanEqual,
	}

	public class FieldAutomationCondition : AutomationCondition
	{
		public const string TypeCode = "field";

		public override string Type => TypeCode;

		public string Field { get; set; }

		public AutomationConditionOperator Operator { get; set; }

		public string Value { get; set; }
	}

	public abstract class AutomationAction
	{
		public abstract string Type { get; }
	}

	public class SetFieldAutomationAction : AutomationAction
	{
		public const string TypeCode = "set-field";

		public override string Type => TypeCode;

		public string Field { get; set; }

		public string Value { get; set; }
	}

	public class NotifyByEmailAutomationAction : AutomationAction
	{
		public const string TypeCode = "notify-by-email";

		public override string Type => TypeCode;

		public string Recipient { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
	}
}

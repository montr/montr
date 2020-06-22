using System;
using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public abstract class AutomationCondition
	{
		public abstract string Type { get; }

		public abstract Type GetPropertiesType();

		public abstract object GetProperties();

		public abstract void SetProperties(object value);
	}

	public abstract class AutomationCondition<TProps> : AutomationCondition where TProps : new()
	{
		protected AutomationCondition()
		{
			Props= new TProps();
		}

		public TProps Props { get; set; }

		public override Type GetPropertiesType()
		{
			return typeof(TProps);
		}

		public override object GetProperties()
		{
			return Props;
		}

		public override void SetProperties(object value)
		{
			Props = (TProps)value;
		}
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

	public class GroupAutomationCondition : AutomationCondition<GroupAutomationCondition.Properties>
	{
		public const string TypeCode = "group";

		public override string Type => TypeCode;

		public class Properties
		{
			public AutomationConditionMeet Meet { get; set; }

			public IList<AutomationCondition> Conditions { get; set; }
		}
	}

	public class FieldAutomationCondition : AutomationCondition<FieldAutomationCondition.Properties>
	{
		public const string TypeCode = "field";

		public override string Type => TypeCode;

		public class Properties
		{
			public string Field { get; set; }

			public AutomationConditionOperator Operator { get; set; }

			public string Value { get; set; }
		}
	}
}

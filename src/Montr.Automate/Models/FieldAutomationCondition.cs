namespace Montr.Automate.Models
{
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

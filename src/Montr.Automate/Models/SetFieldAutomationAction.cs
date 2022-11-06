namespace Montr.Automate.Models
{
	public class SetFieldAutomationAction : AutomationAction<SetFieldAutomationAction.Properties>
	{
		public const string TypeCode = "set-field";

		public override string Type => TypeCode;

		public class Properties
		{
			public string Field { get; set; }

			public string Value { get; set; }
		}
	}
}

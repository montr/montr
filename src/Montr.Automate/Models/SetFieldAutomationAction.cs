using Montr.Automate.Models;

namespace Montr.Automate.Impl.Models
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

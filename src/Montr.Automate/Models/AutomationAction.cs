namespace Montr.Automate.Models
{
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

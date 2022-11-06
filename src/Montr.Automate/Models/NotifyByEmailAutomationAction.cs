namespace Montr.Automate.Models
{
	public class NotifyByEmailAutomationAction : AutomationAction<NotifyByEmailAutomationAction.Properties>
	{
		public const string TypeCode = "notify-by-email";

		public override string Type => TypeCode;

		public class Properties
		{
			public string Recipient { get; set; }

			public string Subject { get; set; }

			public string Body { get; set; }
		}
	}
}

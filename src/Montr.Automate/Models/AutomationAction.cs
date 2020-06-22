using System;

namespace Montr.Automate.Models
{
	public abstract class AutomationAction
	{
		public abstract string Type { get; }

		public abstract Type GetPropertiesType();

		public abstract object GetProperties();

		public abstract void SetProperties(object value);
	}

	public abstract class AutomationAction<TProps> : AutomationAction where TProps : new()
	{
		protected AutomationAction()
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

using Montr.Automate.Models;

namespace Montr.Tasks.Models
{
	public class CreateTaskAutomationAction : AutomationAction<CreateTaskAutomationAction.Properties>
	{
		public const string TypeCode = "create-task";

		public override string Type => TypeCode;

		public class Properties
		{
			public string Name { get; set; }

			public string Description { get; set; }
		}
	}
}

using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public class Automation
	{
		public IList<AutomationCondition> Conditions { get; set; }

		public IList<AutomationAction> Actions { get; set; }
	}

	public class AutomationCondition
	{
		public AutomationConditionMeet Meet { get; set; }
	}

	public enum AutomationConditionMeet
	{
		All,
		Any
	}

	public class AutomationAction
	{
	}
}

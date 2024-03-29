﻿using System.Collections.Generic;

namespace Montr.Automate.Models
{
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
}

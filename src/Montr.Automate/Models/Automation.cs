using System;
using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public class Automation
	{
		public Guid Uid { get; set; }

		public int DisplayOrder { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool Active { get; set; }

		public bool System { get; set; }

		/// <summary>
		/// Meet all conditions...
		/// </summary>
		public IList<AutomationCondition> Conditions { get; set; }

		/// <summary>
		/// ...to execute all actions
		/// </summary>
		public IList<AutomationAction> Actions { get; set; }
	}
}

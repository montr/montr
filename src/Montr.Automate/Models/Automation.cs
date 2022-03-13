using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Models;

namespace Montr.Automate.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Automation : Classifier
	{
		private string DebuggerDisplay => $"{Name} ({EntityTypeCode})";

		/// <summary>
		/// document, classifier etc.</summary>
		public string EntityTypeCode { get; set; }

		public string TypeCode { get; set; }

		/// <summary>
		/// Meet all conditions...
		/// </summary>
		public IList<AutomationCondition> Conditions { get; set; }

		/// <summary>
		/// ...to execute all actions
		/// </summary>
		public IList<AutomationAction> Actions { get; set; }
	}

	public static class AutomationTypeCode
	{
		public static readonly string Trigger = "trigger";
	}
}

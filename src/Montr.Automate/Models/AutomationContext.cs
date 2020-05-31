using System;

namespace Montr.Automate.Models
{
	public class AutomationContext
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityTypeUid { get; set; }

		public object Entity { get; set; }
	}
}

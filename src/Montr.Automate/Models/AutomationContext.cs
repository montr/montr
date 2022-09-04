using System;
using System.Collections.Generic;

namespace Montr.Automate.Models
{
	public class AutomationContext
	{
		public AutomationContext()
		{
			Values = new Dictionary<string, object>();
		}

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IDictionary<string, object> Values { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Montr.Automate.Models
{
	public class AutomationContext
	{
		public AutomationContext()
		{
			Values = new Dictionary<string, object>();
		}

		public string MetadataEntityTypeCode { get; set; }

		public Guid MetadataEntityUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IDictionary<string, object> Values { get; }
	}
}

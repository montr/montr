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

		// todo: EntityTypeCode (?)
		public string MetadataEntityTypeCode { get; set; }

		// todo: remove (?)
		public Guid MetadataEntityUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IDictionary<string, object> Values { get; }
	}
}

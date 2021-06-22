using System;
using Montr.Core.Models;

namespace Montr.Automate.Models
{
	public class AutomationSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public bool IncludeRules { get; set; }

		public Guid? Uid { get; set; }

		public bool? IsActive { get; set; }

		public bool? IsSystem { get; set; }
	}
}

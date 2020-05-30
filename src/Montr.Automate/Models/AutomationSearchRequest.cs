using System;
using Montr.Core.Models;

namespace Montr.Automate.Models
{
	public class AutomationSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }
	}
}

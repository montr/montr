using System;

namespace Montr.Metadata.Models
{
	public class DeleteFieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid[] EntityUids { get; set; }
	}
}

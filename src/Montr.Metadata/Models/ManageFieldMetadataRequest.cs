using System;

namespace Montr.Metadata.Models
{
	public class ManageFieldMetadataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public FieldMetadata Item { get; set; }
	}
}

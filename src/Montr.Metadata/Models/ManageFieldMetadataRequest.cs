using System;
using System.Collections.Generic;

namespace Montr.Metadata.Models
{
	public class ManageFieldMetadataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IEnumerable<FieldMetadata> Items { get; set; }
	}
}

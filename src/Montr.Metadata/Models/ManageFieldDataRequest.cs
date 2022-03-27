using System;
using System.Collections.Generic;

namespace Montr.Metadata.Models
{
	public class ManageFieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public ICollection<FieldMetadata> Metadata { get; set; }

		public IFieldDataContainer Item { get; set; }
	}
}

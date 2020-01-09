using System;
using System.Collections.Generic;
using Montr.Metadata.Services;

namespace Montr.Metadata.Models
{
	public class ManageFieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IList<FieldMetadata> Metadata { get; set; }

		public IFieldDataContainer Item { get; set; }
	}
}

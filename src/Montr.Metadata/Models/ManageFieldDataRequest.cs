using System;
using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Metadata.Models
{
	public class ManageFieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IList<FieldMetadata> Metadata { get; set; }

		public FieldData Data { get; set; }
	}
}

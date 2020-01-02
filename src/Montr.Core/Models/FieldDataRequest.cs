using System;
using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class FieldDataSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid[] EntityUids { get; set; }
	}

	public class FieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IList<FieldMetadata> Metadata { get; set; }

		public FieldData Data { get; set; }
	}

	public class DeleteFieldDataRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid[] EntityUids { get; set; }
	}
}

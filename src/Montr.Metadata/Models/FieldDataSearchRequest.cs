using System;
using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Metadata.Models
{
	public class FieldDataSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid[] EntityUids { get; set; }

		public IList<FieldMetadata> Metadata { get; set; }
	}
}

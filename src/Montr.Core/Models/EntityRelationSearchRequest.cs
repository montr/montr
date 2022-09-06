using System;

namespace Montr.Core.Models
{
	public class EntityRelationSearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }
	}
}

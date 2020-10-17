using System;

namespace Montr.Core.Models
{
	public class EntityStatusSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid? Uid { get; set; }
	}
}

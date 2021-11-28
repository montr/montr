using System;

namespace Montr.Core.Models;

public class EntityRelation
{
	public string EntityTypeCode { get; set; }

	public Guid EntityUid { get; set; }
		
	public string RelatedEntityTypeCode { get; set; }

	public Guid RelatedEntityUid { get; set; }

	public string RelationType { get; set; }
}

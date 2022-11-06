using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Entities
{
	[Table(Schema = "montr", Name = "entity_relation")]
	public class DbEntityRelation
	{
		[Column(Name = "entity_type_code"), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public Guid EntityUid { get; set; }

		[Column(Name = "related_entity_type_code"), DataType(DataType.VarChar), NotNull]
		public string RelatedEntityTypeCode { get; set; }

		[Column(Name = "related_entity_uid"), DataType(DataType.Guid), NotNull]
		public Guid RelatedEntityUid { get; set; }

		[Column(Name = "relation_type"), DataType(DataType.VarChar), NotNull]
		public string RelationType { get; set; }
	}
}

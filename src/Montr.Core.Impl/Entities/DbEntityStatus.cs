using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities
{
	[Table(Schema = "montr", Name = "entity_status")]
	public class DbEntityStatus
	{
		[Column(Name = "entity_type_code"), DataType(DataType.VarChar), NotNull, PrimaryKey(0)]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(1)]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull, PrimaryKey(2)]
		public string Code { get; set; }

		[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
		public int DisplayOrder { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), NotNull]
		public string Name { get; set; }
	}
}

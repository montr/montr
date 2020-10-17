using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities
{
	[Table(Schema = "montr", Name = "entity_status")]
	public class DbEntityStatus
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public System.Guid Uid { get; set; }

		[Column(Name = "entity_type_code"), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
		public int DisplayOrder { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), NotNull]
		public string Name { get; set; }
	}
}

using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities
{
	[Table(Schema = "public", Name = "field_data")]
	public class DbFieldData
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public System.Guid Uid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "key", Length = 32), DataType(DataType.VarChar), NotNull]
		public string Key { get; set; }

		[Column(Name = "value"), DataType(DataType.NText)]
		public string Value { get; set; }
	}
}

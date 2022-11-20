using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Settings.Entities
{
	[Table(Schema = "montr", Name = "settings")]
	public class DbSettings
	{
		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull, PrimaryKey(0)]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(1)]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "key"), DataType(DataType.VarChar), NotNull, PrimaryKey(2)]
		public string Key { get; set; }

		[Column(Name = "value"), DataType(DataType.VarChar)]
		public string Value { get; set; }

		[Column(Name = "created_by"), DataType(DataType.Guid)]
		public System.Guid CreatedBy { get; set; }

		[Column(Name = "created_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime CreatedAtUtc { get; set; }

		[Column(Name = "modified_by"), DataType(DataType.Guid)]
		public System.Guid ModifiedBy { get; set; }

		[Column(Name = "modified_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime ModifiedAtUtc { get; set; }
	}
}

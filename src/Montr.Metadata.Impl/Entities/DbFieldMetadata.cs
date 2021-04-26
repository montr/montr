using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Metadata.Impl.Entities
{
	[Table(Schema = "montr", Name = "field_metadata")]
	public class DbFieldMetadata
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey]
		public System.Guid Uid { get; set; }

		[Column(Name = "entity_type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string EntityTypeCode { get; set; }

		[Column(Name = "entity_uid"), DataType(DataType.Guid), NotNull]
		public System.Guid EntityUid { get; set; }

		[Column(Name = "key", Length = 32), DataType(DataType.VarChar) /*, NotNull*/]
		public string Key { get; set; }

		[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
		public string Type { get; set; }

		[Column(Name = "is_active"), DataType(DataType.Boolean), NotNull]
		public bool IsActive { get; set; }

		[Column(Name = "is_system"), DataType(DataType.Boolean), NotNull]
		public bool IsSystem { get; set; }

		[Column(Name = "is_required"), DataType(DataType.Boolean), NotNull]
		public bool IsRequired { get; set; }

		[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
		public int DisplayOrder { get; set; }

		[Column(Name = "is_readonly"), DataType(DataType.Boolean), NotNull]
		public bool IsReadonly { get; set; }

		[Column(Name = "created_by"), DataType(DataType.Guid)]
		public System.Guid? CreatedBy { get; set; }

		[Column(Name = "created_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime? CreatedAtUtc { get; set; }

		[Column(Name = "modified_by"), DataType(DataType.Guid)]
		public System.Guid? ModifiedBy { get; set; }

		[Column(Name = "modified_at_utc"), DataType(DataType.DateTime2)]
		public System.DateTime? ModifiedAtUtc { get; set; }

		[Column(Name = "name", Length = 128), DataType(DataType.VarChar)]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar)]
		public string Description { get; set; }

		[Column(Name = "placeholder", Length = 128), DataType(DataType.VarChar)]
		public string Placeholder { get; set; }

		[Column(Name = "icon", Length = 32), DataType(DataType.VarChar)]
		public string Icon { get; set; }

		[Column(Name = "props"), DataType(DataType.NText)]
		public string Props { get; set; }
	}
}

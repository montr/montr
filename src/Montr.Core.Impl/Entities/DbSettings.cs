using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Core.Impl.Entities
{
	[Table(Schema = "montr", Name = "settings")]
	public class DbSettings
	{
		[Column(Name = "id"), DataType(DataType.VarChar), NotNull, PrimaryKey]
		public string Id { get; set; }

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

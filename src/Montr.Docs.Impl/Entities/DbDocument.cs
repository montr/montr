using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Impl.Entities
{
	[Table(Schema = "montr", Name = "document")]
	public class DbDocument
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), NotNull]
		public Guid CompanyUid { get; set; }

		[Column(Name = "config_code"), DataType(DataType.VarChar), NotNull]
		public string ConfigCode { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "direction"), DataType(DataType.VarChar), NotNull]
		public string Direction { get; set; }

		[Column(Name = "document_number"), DataType(DataType.VarChar), NotNull]
		public string DocumentNumber { get; set; }

		[Column("document_date_utc"), DataType(DataType.DateTime2), NotNull]
		public DateTime DocumentDate { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }

		[Column("created_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? CreatedAtUtc { get; set; }

		[Column("created_by")]
		public string CreatedBy { get; set; }

		[Column("modified_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? ModifiedAtUtc { get; set; }

		[Column("modified_by")]
		public string ModifiedBy { get; set; }
	}
}

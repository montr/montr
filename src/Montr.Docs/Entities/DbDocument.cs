﻿using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Entities
{
	[Table(Schema = "montr", Name = "document")]
	public class DbDocument
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "document_type_uid"), DataType(DataType.Guid), NotNull]
		public Guid DocumentTypeUid { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "direction"), DataType(DataType.VarChar), NotNull]
		public string Direction { get; set; }

		[Column("document_date_utc"), DataType(DataType.DateTime2), NotNull]
		public DateTime DocumentDate { get; set; }

		[Column(Name = "document_number"), DataType(DataType.VarChar), Nullable]
		public string DocumentNumber { get; set; }

		[Column(Name = "company_uid"), DataType(DataType.Guid), Nullable]
		public Guid? CompanyUid { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }

		[Column("created_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? CreatedAtUtc { get; set; }

		[Column("created_by")]
		public Guid? CreatedBy { get; set; }

		[Column("modified_at_utc"), DataType(DataType.DateTime2)]
		public DateTime? ModifiedAtUtc { get; set; }

		[Column("modified_by")]
		public Guid? ModifiedBy { get; set; }
	}
}
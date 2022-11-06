using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Entities;

[Table(Schema = "montr", Name = "document_type")]
public class DbDocumentType
{
	[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
	public Guid Uid { get; set; }
}
using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Impl.Entities
{
	[Table(Schema = "montr", Name = "document_type")]
	public class DbDocumentType
	{
		[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), NotNull]
		public string Name { get; set; }

		[Column(Name = "description"), DataType(DataType.VarChar)]
		public string Description { get; set; }
	}
}

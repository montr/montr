using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Docs.Entities;

[Table(Schema = "montr", Name = "process_step")]
public class DbProcessStep
{
	[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
	public Guid Uid { get; set; }

	[Column(Name = "process_uid"), DataType(DataType.Guid), NotNull]
	public Guid ProcessUid { get; set; }

	[Column(Name = "display_order"), DataType(DataType.Int32), NotNull]
	public int DisplayOrder { get; set; }

	[Column(Name = "type_code", Length = 32), DataType(DataType.VarChar), NotNull]
	public string TypeCode { get; set; }

	[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
	public string Name { get; set; }

	[Column(Name = "description"), DataType(DataType.VarChar), Nullable]
	public string Description { get; set; }

	[Column(Name = "props"), DataType(DataType.NText)]
	public string Props { get; set; }
}
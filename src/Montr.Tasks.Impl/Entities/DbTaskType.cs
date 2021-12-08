using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.Tasks.Impl.Entities;

[Table(Schema = "montr", Name = "task_type")]
public class DbTaskType
{
	[Column(Name = "uid"), DataType(DataType.Guid), NotNull, PrimaryKey, Identity]
	public Guid Uid { get; set; }
}

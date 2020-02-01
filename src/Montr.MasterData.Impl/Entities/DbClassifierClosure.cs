using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	/// <summary>
	/// Closure table for group/group or item/item relation
	/// </summary>
	[Table(Schema = "montr", Name = "classifier_closure")]
	public class DbClassifierClosure
	{
		[Column(Name = "parent_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(0)]
		public Guid ParentUid { get; set; }

		[Column(Name = "child_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(1)]
		public Guid ChildUid { get; set; }

		[Column(Name = "level"), DataType(DataType.Int16), NotNull, PrimaryKey(2)]
		public short Level { get; set; }
	}
}

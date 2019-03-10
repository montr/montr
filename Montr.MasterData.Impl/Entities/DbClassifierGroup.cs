using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Name = "classifier_group")]
	public class DbClassifierGroup
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey, Identity]
		public Guid Uid { get; set; }

		[Column(Name = "tree_uid"), DataType(DataType.Guid), NotNull]
		public Guid TreeUid { get; set; }

		[Column(Name = "parent_uid"), DataType(DataType.Guid), Nullable]
		public Guid? ParentUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }
	}
}

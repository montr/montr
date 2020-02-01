using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Schema = "montr", Name = "classifier_tree")]
	public class DbClassifierTree
	{
		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "type_uid"), DataType(DataType.Guid), NotNull]
		public Guid TypeUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }
	}
}

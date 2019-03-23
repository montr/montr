using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Name = "classifier")]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DbClassifier
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		[Column(Name = "uid"), DataType(DataType.Guid), PrimaryKey]
		public Guid Uid { get; set; }

		[Column(Name = "type_uid"), DataType(DataType.Guid), NotNull]
		public Guid TypeUid { get; set; }

		[Column(Name = "status_code"), DataType(DataType.VarChar), NotNull]
		public string StatusCode { get; set; }

		[Column(Name = "parent_uid"), DataType(DataType.Guid), Nullable]
		public Guid? ParentUid { get; set; }

		[Column(Name = "code"), DataType(DataType.VarChar), NotNull]
		public string Code { get; set; }

		[Column(Name = "name"), DataType(DataType.VarChar), Nullable]
		public string Name { get; set; }
	}
}

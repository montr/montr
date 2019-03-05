using System;
using LinqToDB;
using LinqToDB.Mapping;

namespace Montr.MasterData.Impl.Entities
{
	[Table(Name = "classifier_link")]
	public class DbClassifierLink
	{
		[Column(Name = "group_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(0)]
		public Guid GroupUid { get; set; }

		[Column(Name = "item_uid"), DataType(DataType.Guid), NotNull, PrimaryKey(1)]
		public Guid ItemUid { get; set; }
	}
}

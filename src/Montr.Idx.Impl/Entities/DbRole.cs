using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Impl.Entities
{
	[Table(Schema = "montr", Name = "roles")]
	public class DbRole : IdentityRole<Guid>
	{
		[Column("id"), PrimaryKey, /*Identity,*/ NotNull]
		public override Guid Id { get; set; }

		[Column("name", Length = 128)]
		public override string Name { get; set; }

		[Column("normalized_name", Length = 128)]
		public override string NormalizedName { get; set; }

		[Column("concurrency_stamp", Length = 36), NotNull]
		public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
	}
}

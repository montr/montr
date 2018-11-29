﻿using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Idx.Entities
{
	[Table(Schema = "public", Name = "roles")]
	public class DbRole : IdentityRole<Guid>
	{
		[Column("id"), PrimaryKey, /*Identity,*/ NotNull]
		public override Guid Id { get; set; }

		[Column("name")]
		public override string Name { get; set; }

		[Column("normalized_name")]
		public override string NormalizedName { get; set; }

		[Column("concurrency_stamp"), NotNull]
		public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
	}
}

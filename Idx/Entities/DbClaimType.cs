using System;
using LinqToDB.Mapping;

namespace Idx.Entities
{
	[Table(Schema = "public", Name = "claim_type")]
	public class DbClaimType
	{
		[Column("id"), PrimaryKey(Order = 0)]
		public Guid Id { get; set; }

		[Column("code"), NotNull]
		public string Code { get; set; }

		[Column("uri"), NotNull]
		public string Uri { get; set; }
	}
}
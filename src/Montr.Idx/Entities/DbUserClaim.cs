using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_claim")]
	public class DbUserClaim : IdentityUserClaim<Guid>
	{
		[Column("id"), PrimaryKey, NotNull]
		public override int Id { get; set; }

		[Column("user_id"), NotNull]
		public override Guid UserId { get; set; }

		[Column("claim_type", Length = 32), NotNull]
		public override string ClaimType { get; set; }

		[Column("claim_value", Length = 128), NotNull]
		public override string ClaimValue { get; set; }
	}
}

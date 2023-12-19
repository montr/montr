using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "role_claim")]
	public class DbRoleClaim : IdentityRoleClaim<Guid>
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public override int Id { get; set; }

		[Column("role_id"), NotNull]
		public override Guid RoleId { get; set; }

		[Column("claim_type", Length = 32), NotNull]
		public override string ClaimType { get; set; }

		[Column("claim_value", Length = 128), NotNull]
		public override string ClaimValue { get; set; }
	}
}

using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Impl.Entities
{
	[Table(Schema = "montr", Name = "role_claim")]
	public class DbRoleClaim // : IdentityRoleClaim<Guid>
	{
		[Column("id"), NotNull, PrimaryKey]
		public Guid Id { get; set; }

		[Column("role_id"), NotNull]
		public Guid RoleId { get; set; }

		[Column("claim_type", Length = 32), NotNull]
		public string ClaimType { get; set; }

		[Column("claim_value", Length = 128), NotNull]
		public string ClaimValue { get; set; }
	}
}

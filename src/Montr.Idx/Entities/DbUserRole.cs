using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_role")]
	public class DbUserRole : IdentityUserRole<Guid>
	{
		[Column("user_id"), PrimaryKey(0), NotNull]
		public override Guid UserId { get; set; }

		[Column("role_id"), PrimaryKey(1), NotNull]
		public override Guid RoleId { get; set; }
	}
}

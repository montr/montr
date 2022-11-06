using System;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_role")]
	public class DbUserRole
	{
		[Column("user_id"), PrimaryKey(0), NotNull]
		public Guid UserId { get; set; }

		[Column("role_id"), PrimaryKey(1), NotNull]
		public Guid RoleId { get; set; }
	}
}

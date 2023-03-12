using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_token")]
	public class DbUserToken : IdentityUserToken<Guid>
	{
		[Column("user_id"), PrimaryKey(0), NotNull]
		public override Guid UserId { get; set; }

		[Column("login_provider", Length = 36), PrimaryKey(1), NotNull]
		public override string LoginProvider { get; set; }

		[Column("name", Length = 128), PrimaryKey(2), NotNull]
		public override string Name { get; set; }

		[Column("value"), NotNull]
		public override string Value { get; set; }
	}
}

using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "user_login")]
	public class DbUserLogin : IdentityUserLogin<Guid>
	{
		[Column("user_id"), NotNull]
		public override Guid UserId { get; set; }

		[Column("login_provider", Length = 36), PrimaryKey(0), NotNull]
		public override string LoginProvider { get; set; }

		[Column("provider_key", Length = 128), PrimaryKey(1), NotNull]
		public override string ProviderKey { get; set; }

		[Column("provider_display_name", Length = 128), NotNull]
		public override string ProviderDisplayName { get; set; }
	}
}

using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Montr.Idx.Entities
{
	[Table(Schema = "montr", Name = "users")]
	public class DbUser : IdentityUser<Guid>
	{
		[Column("id"), PrimaryKey, /*Identity,*/ NotNull]
		public override Guid Id { get; set; }

		[Column("user_name", Length = 128)]
		public override string UserName { get; set; }

		[Column("normalized_user_name", Length = 128)]
		public override string NormalizedUserName { get; set; }

		[Column("first_name")]
		public string FirstName { get; set; }

		[Column("last_name")]
		public string LastName { get; set; }

		[Column("email", Length = 128)]
		public override string Email { get; set; }

		[Column("normalized_email", Length = 128)]
		public override string NormalizedEmail { get; set; }

		[Column("email_confirmed"), NotNull]
		public override bool EmailConfirmed { get; set; }

		[Column("phone_number", Length = 12)]
		public override string PhoneNumber { get; set; }

		[Column("phone_number_confirmed"), NotNull]
		public override bool PhoneNumberConfirmed { get; set; }

		[Column("password_hash")]
		public override string PasswordHash { get; set; }

		[Column("security_stamp")]
		public override string SecurityStamp { get; set; }

		[Column("two_factor_enabled"), NotNull]
		public override bool TwoFactorEnabled { get; set; }

		[Column("lockout_enabled"), NotNull]
		public override bool LockoutEnabled { get; set; }

		[Column("lockout_end")]
		public override DateTimeOffset? LockoutEnd { get; set; }

		[Column("access_failed_count"), NotNull]
		public override int AccessFailedCount { get; set; }

		[Column("concurrency_stamp", Length = 36), NotNull]
		public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

		/*[Column("created_at"), NotNull]
		public DateTime? CreatedAt { get; set; }

		[Column("created_by")]
		public string CreatedBy { get; set; }

		[Column("modified_at")]
		public DateTime? ModifiedAt { get; set; }

		[Column("modified_by")]
		public string ModifiedBy { get; set; }*/
	}
}

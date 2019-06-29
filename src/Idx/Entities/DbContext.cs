using System;
using LinqToDB;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Idx.Entities
{
	public class DbContext : DataContext
	{
		/*public DbContext()
		{
			// https://github.com/linq2db/linq2db/issues/286
			// https://github.com/linq2db/t4models

			// MapSchema(MappingSchema);
		}*/

		public static void MapSchema(MappingSchema mappingSchema)
		{
			mappingSchema.GetFluentMappingBuilder()

				.Entity<IdentityRoleClaim<Guid>>().HasTableName("role_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey()
				.Property(x => x.RoleId).HasColumnName("role_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)
				
				.Entity<IdentityUserClaim<Guid>>().HasTableName("user_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey().IsIdentity()
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)
				
				.Entity<IdentityUserLogin<Guid>>().HasTableName("user_login")
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(0)
				.Property(x => x.ProviderKey).HasColumnName("provider_key").IsPrimaryKey(1)
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name")
				
				.Entity<IdentityUserRole<Guid>>().HasTableName("user_role")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.RoleId).HasColumnName("role_id").IsPrimaryKey(1)
				
				.Entity<IdentityUserToken<Guid>>().HasTableName("user_token")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(1)
				.Property(x => x.Name).HasColumnName("name").IsPrimaryKey(2)
				.Property(x => x.Value).HasColumnName("value");
		}
	}
}
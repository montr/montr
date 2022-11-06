using System;
using LinqToDB.Mapping;

namespace Montr.Idx.Services.Implementations
{
	public static class IdentitySchemaMapper
	{
		public static void MapSchema(MappingSchema mappingSchema)
		{
			mappingSchema.GetFluentMappingBuilder()

				.Entity<LinqToDB.Identity.IdentityRoleClaim<Guid>>().HasSchemaName("montr").HasTableName("role_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey().IsIdentity()
				.Property(x => x.RoleId).HasColumnName("role_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserClaim<Guid>>().HasSchemaName("montr").HasTableName("user_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey().IsIdentity()
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserLogin<Guid>>().HasSchemaName("montr").HasTableName("user_login")
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(0)
				.Property(x => x.ProviderKey).HasColumnName("provider_key").IsPrimaryKey(1)
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name")

				.Entity<LinqToDB.Identity.IdentityUserRole<Guid>>().HasSchemaName("montr").HasTableName("user_role")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.RoleId).HasColumnName("role_id").IsPrimaryKey(1)

				.Entity<LinqToDB.Identity.IdentityUserToken<Guid>>().HasSchemaName("montr").HasTableName("user_token")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(1)
				.Property(x => x.Name).HasColumnName("name").IsPrimaryKey(2)
				.Property(x => x.Value).HasColumnName("value");
		}
	}
}

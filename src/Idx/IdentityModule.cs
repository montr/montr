using System;
using Idx.Services;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Idx.Impl.Entities;
using Montr.Modularity;

namespace Idx
{
	public class IdentityModule : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			/*services.Configure<IdentityOptions>(options =>
			{
				options.User.RequireUniqueEmail = false;

				/* // Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; #1#
			});

			// https://github.com/linq2db/linq2db/issues/286
			// https://github.com/linq2db/t4models
			MapSchema(MappingSchema.Default);

			services
				.AddIdentity<DbUser, DbRole>()
				.AddLinqToDBStores(new DbConnectionFactory(),
					typeof(Guid),
					typeof(LinqToDB.Identity.IdentityUserClaim<Guid>),
					typeof(LinqToDB.Identity.IdentityUserRole<Guid>),
					typeof(LinqToDB.Identity.IdentityUserLogin<Guid>),
					typeof(LinqToDB.Identity.IdentityUserToken<Guid>),
					typeof(LinqToDB.Identity.IdentityRoleClaim<Guid>))
				.AddDefaultTokenProviders();*/

			// using Microsoft.AspNetCore.Identity.UI.Services;
			services.AddSingleton<IEmailSender, EmailSender>();
		}

		public static void MapSchema(MappingSchema mappingSchema)
		{
			mappingSchema.GetFluentMappingBuilder()

				.Entity<LinqToDB.Identity.IdentityRoleClaim<Guid>>().HasTableName("role_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey()
				.Property(x => x.RoleId).HasColumnName("role_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserClaim<Guid>>().HasTableName("user_claim")
				.Property(x => x.Id).HasColumnName("id").IsPrimaryKey().IsIdentity()
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ClaimType).HasColumnName("claim_type").IsNullable(false)
				.Property(x => x.ClaimValue).HasColumnName("claim_value").IsNullable(false)

				.Entity<LinqToDB.Identity.IdentityUserLogin<Guid>>().HasTableName("user_login")
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(0)
				.Property(x => x.ProviderKey).HasColumnName("provider_key").IsPrimaryKey(1)
				.Property(x => x.UserId).HasColumnName("user_id").IsNullable(false)
				.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name")

				.Entity<LinqToDB.Identity.IdentityUserRole<Guid>>().HasTableName("user_role")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.RoleId).HasColumnName("role_id").IsPrimaryKey(1)

				.Entity<LinqToDB.Identity.IdentityUserToken<Guid>>().HasTableName("user_token")
				.Property(x => x.UserId).HasColumnName("user_id").IsPrimaryKey(0)
				.Property(x => x.LoginProvider).HasColumnName("login_provider").IsPrimaryKey(1)
				.Property(x => x.Name).HasColumnName("name").IsPrimaryKey(2)
				.Property(x => x.Value).HasColumnName("value");
		}
	}
}

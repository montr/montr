using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Montr.Idx.Entities
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			// fix for Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes.
			// See the inner exception for details.
			// ---> System.InvalidCastException: Cannot write DateTime with Kind=Unspecified to PostgreSQL type
			// 'timestamp with time zone', only UTC is supported. Note that it's not possible to mix DateTimes
			// with different Kinds in an array/range. See the Npgsql.EnableLegacyTimestampBehavior AppContext switch
			// to enable legacy behavior.
			//
			// see https://github.com/openiddict/openiddict-core/issues/1376#issuecomment-1151275376
			configurationBuilder.Properties<DateTime>().HaveConversion<UtcValueConverter>();
			configurationBuilder.Properties<DateTime?>().HaveConversion<UtcValueConverter>();

			base.ConfigureConventions(configurationBuilder);
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class UtcValueConverter : ValueConverter<DateTime, DateTime>
		{
			public UtcValueConverter() : base(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
			{
			}
		}
	}
}

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Montr.Core.Impl.Entities;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbConfigurationSource : IConfigurationSource
	{
		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			// todo: resolve services
			var dbContextFactory = new DefaultDbContextFactory();

			return new DbConfigurationProvider(dbContextFactory);
		}
	}

	public class DbConfigurationProvider : ConfigurationProvider
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbConfigurationProvider(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public override void Load()
		{
			Console.WriteLine("DbConfigurationProvider.Load");

			using (var db = _dbContextFactory.Create())
			{
				Data = db.GetTable<DbSettings>()
					.ToDictionary(x => x.Id, x => x.Value, StringComparer.OrdinalIgnoreCase);
			}
		}
	}

	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder)
		{
			return builder.Add(new DbConfigurationSource());
		}
	}
}

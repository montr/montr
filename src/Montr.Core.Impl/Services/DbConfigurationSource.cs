using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Montr.Core.Impl.Entities;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbConfigurationSource : IConfigurationSource
	{
		public bool ReloadOnChange { get; set; }

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
			using (var db = _dbContextFactory.Create())
			{
				Data = db.GetTable<DbSettings>()
					.ToDictionary(x => x.Id, x => x.Value, StringComparer.OrdinalIgnoreCase);
			}

			// OnReload();
		}
	}

	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationBuilder AddDbSettings(this IConfigurationBuilder builder, bool reloadOnChange = true)
		{
			return builder.Add(new DbConfigurationSource { ReloadOnChange = reloadOnChange });
		}
	}
}

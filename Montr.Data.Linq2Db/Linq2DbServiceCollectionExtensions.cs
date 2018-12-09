using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Data.Linq2Db
{
	public class ConnectionStringSettings : IConnectionStringSettings
	{
		public string ConnectionString { get; set; }

		public string Name { get; set; }

		public string ProviderName { get; set; }

		public bool IsGlobal => false;
	}

	public class DbSettings : ILinqToDBSettings
	{
		private readonly IConnectionStringSettings _connectionStringSettings;

		public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

		public string DefaultConfiguration => "Default";
		public string DefaultDataProvider => "PostgreSQL"; // "SqlServer";

		public DbSettings(IConnectionStringSettings connectionStringSettings)
		{
			_connectionStringSettings = connectionStringSettings;
		}

		public IEnumerable<IConnectionStringSettings> ConnectionStrings
		{
			get
			{
				yield return _connectionStringSettings;
			}
		}
	}

	public static class Linq2DbServiceCollectionExtensions
	{
		public static void AddLinq2Db(this IServiceCollection services, IConnectionStringSettings connectionStringSettings)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			DataConnection.DefaultSettings = new DbSettings(connectionStringSettings);
		}
	}
}

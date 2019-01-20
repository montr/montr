using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Montr.Data.Linq2Db
{
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
}
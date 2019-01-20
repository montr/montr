using System;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Data.Linq2Db
{
	public static class Linq2DbServiceCollectionExtensions
	{
		public static void AddLinq2Db(this IServiceCollection services, IConnectionStringSettings connectionStringSettings)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			DataConnection.DefaultSettings = new DbSettings(connectionStringSettings);
		}
	}
}

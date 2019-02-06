using System;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;

namespace Montr.Data.Linq2Db
{
	public static class DbConfigurationExtensions
	{
		public static void SetLinq2DbTestSettings(string jsonPath = "appsettings.json")
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile(jsonPath)
				// .AddUserSecrets(...)
				.AddEnvironmentVariables()
				.Build();

			configuration.SetLinq2DbDefaultSettings();
		}

		public static void SetLinq2DbDefaultSettings(this IConfiguration configuration, string sectionName = "ConnectionString")
		{
			if (configuration == null) throw new ArgumentNullException(nameof(configuration));

			var connectionStringSettings = configuration
				.GetSection(sectionName).Get<ConnectionStringSettings>();

			DataConnection.DefaultSettings = new DbSettings(connectionStringSettings);
		}
	}
}

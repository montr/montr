using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Montr.Core;
using Newtonsoft.Json;

namespace Montr.Worker.Hangfire.Services
{
	public static class HangfireGlobalConfigurationExtensions
	{
		public static void UseDefaults(this IGlobalConfiguration config, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(Constants.DefaultConnectionStringName);

			config.UsePostgreSqlStorage(options =>
			{
				options.UseNpgsqlConnection(connectionString);
			});

			/*config.UsePostgreSqlStorage(
				connectionString,
				new PostgreSqlStorageOptions
				{
					PrepareSchemaIfNecessary = false,
					EnableTransactionScopeEnlistment = true
				});*/

			config.UseSerializerSettings(
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects
				});
		}
	}
}

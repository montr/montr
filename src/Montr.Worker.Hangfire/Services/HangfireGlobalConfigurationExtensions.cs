using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Montr.Worker.Hangfire.Services;

public static class HangfireGlobalConfigurationExtensions
{
	public static void UseDefaults(this IGlobalConfiguration config, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString(Data.Constants.DefaultConnectionStringName);

		config.UsePostgreSqlStorage(
			connectionString,
			new PostgreSqlStorageOptions
			{
				PrepareSchemaIfNecessary = false,
				EnableTransactionScopeEnlistment = true
			});

		config.UseSerializerSettings(
			new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
	}
}

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Npgsql.Logging;

namespace Montr.Data.Npgsql
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			NpgsqlLogManager.IsParameterLoggingEnabled = true;
			// NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);

			// todo: remove
			// https://github.com/frankhommers/Hangfire.PostgreSql/issues/214
			// https://www.npgsql.org/doc/release-notes/6.0.html#parameter-names-now-use-case-sensitive-matching
			AppContext.SetSwitch("Npgsql.EnableLegacyCaseInsensitiveDbParameters", true);
		}
	}
}

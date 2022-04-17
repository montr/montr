using Microsoft.AspNetCore.Builder;
using Montr.Core;
using Npgsql.Logging;

namespace Montr.Data.Npgsql
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			// todo: remove: use connection string parameters
			NpgsqlLogManager.IsParameterLoggingEnabled = true;
			// NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);
		}
	}
}

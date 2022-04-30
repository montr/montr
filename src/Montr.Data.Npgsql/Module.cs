using Montr.Core;
using Npgsql.Logging;

namespace Montr.Data.Npgsql
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			// todo: remove: use connection string parameters
			NpgsqlLogManager.IsParameterLoggingEnabled = true;
			// NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);
		}
	}
}

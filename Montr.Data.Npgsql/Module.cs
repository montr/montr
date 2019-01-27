using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;
using Npgsql.Logging;

namespace Montr.Data.Npgsql
{
	public class Module: IModule
	{
		public void ConfigureServices(IServiceCollection services)
		{
			NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);
		}
	}
}

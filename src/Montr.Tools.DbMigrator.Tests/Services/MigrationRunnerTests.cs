using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.Tools.DbMigrator.Services;

namespace Montr.Tools.DbMigrator.Tests.Services
{
	[TestClass]
	public class MigrationRunnerTests
	{
		[TestMethod]
		public async Task Run_DefaultConfig_ShouldBootstrapDatabase()
		{
			// arrange
			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd));
			var hashProvider = new DefaultHashProvider();
			var dbContextFactory = new DefaultDbContextFactory();
			var migrator = new MigrationRunner(loggerFactory.CreateLogger<MigrationRunner>(), hashProvider, dbContextFactory);
			var cancellationToken = new CancellationToken();

			// act
			await migrator.Run(new Options
			{
				MigrationPath = "../../../../../sql/"
			}, cancellationToken);

			// assert
		}
	}
}

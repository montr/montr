using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class DbMigrationRunnerTests
	{
		[TestMethod]
		public async Task Run_DefaultConfig_ShouldBootstrapDatabase()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd));
			var dbContextFactory = new DefaultDbContextFactory();
			var migrator = new DbMigrationRunner(loggerFactory.CreateLogger<DbMigrationRunner>(), null, dbContextFactory);
			var migrationOptions = new MigrationOptions
			{
				MigrationPath = "../../../../../sql/"
			};

			// act

			await migrator.Run(cancellationToken);

			// assert
		}
	}
}

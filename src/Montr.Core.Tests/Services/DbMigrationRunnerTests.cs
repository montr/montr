using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;
using Montr.Data.Linq2Db;
using Moq;

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
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd));
			var optionsMonitorMock = new Mock<IOptionsMonitor<MigrationOptions>>();
			optionsMonitorMock.Setup(x => x.CurrentValue).Returns(() => new MigrationOptions
			{
				MigrationPath = "../../../../../database/migrations/"
			});
			var dbContextFactory = new DefaultDbContextFactory();
			var resourceProvider = new EmbeddedResourceProvider();
			var migrator = new DbMigrationRunner(
				loggerFactory.CreateLogger<DbMigrationRunner>(),
				optionsMonitorMock.Object,
				dbContextFactory, resourceProvider);

			// act

			await migrator.Run(cancellationToken);

			// assert
		}
	}
}

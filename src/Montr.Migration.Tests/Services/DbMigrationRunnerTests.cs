using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.Migration.Tests.Services
{
	[TestFixture]
	public class DbMigrationRunnerTests
	{
		[Test]
		public async Task Run_DefaultConfig_ShouldBootstrapDatabase()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var optionsMonitorMock = new Mock<IOptionsMonitor<MigrationOptions>>();
			optionsMonitorMock.Setup(x => x.CurrentValue).Returns(() => new MigrationOptions
			{
				MigrationPath = "../../../../../database/migrations/"
			});
			var dbContextFactory = new DefaultDbContextFactory();
			var resourceProvider = new EmbeddedResourceProvider();
			var migrator = new DbMigrationRunner(
				NullLogger<DbMigrationRunner>.Instance,
				optionsMonitorMock.Object,
				dbContextFactory, resourceProvider);

			// act
			await migrator.Run(cancellationToken);

			// assert
		}
	}
}

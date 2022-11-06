﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Core.Services.Impl;
using Moq;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class DbMigrationRunnerTests
	{
		[Test]
		public async Task Run_DefaultConfig_ShouldBootstrapDatabase()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.FormatterName = "systemd"));
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

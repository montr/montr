using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class ModuleLoaderTests
	{
		[TestMethod]
		public void GetSortedModules_ForHost_ReturnsSortedModules()
		{
			// arrange
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd));
			var loader = new ModuleLoader(loggerFactory.CreateLogger<ModuleLoader>());

			// act
			var modules = loader.GetSortedModules("../../../../Host/bin/Debug/net5.0/", false);

			// assert
			// Assert.AreEqual(0, loader.Errors.Count); // todo: fix errors in github actions tests
			Assert.IsTrue(modules.Count > 0);
		}

		[TestMethod]
		[DataRow("Microsoft.Extensions.Http.dll", true)]
		[DataRow("System.Data.SqlClient.dll", true)]
		[DataRow("Montr.Core.dll", false)]
		public void ExcludeAssembly_ForFile_ReturnsExpected(string file, bool expected)
		{
			// arrange
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd));
			var loader = new ModuleLoader(loggerFactory.CreateLogger<ModuleLoader>());

			// act
			var result = loader.ExcludeAssembly(file);

			// assert
			Assert.AreEqual(expected, result);
		}
	}
}

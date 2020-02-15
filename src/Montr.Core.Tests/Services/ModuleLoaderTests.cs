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
			var modules = loader.GetSortedModules("../../../../Host/bin/Debug/netcoreapp3.1/");

			// assert
			Assert.IsTrue(modules.Count > 20);
		}
	}
}

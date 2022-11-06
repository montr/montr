using Microsoft.Extensions.Logging;
using Montr.Core.Services.Impl;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class ModuleLoaderTests
	{
		[Test]
		public void GetSortedModules_ForHost_ReturnsSortedModules()
		{
			// arrange
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.FormatterName = "systemd"));
			var loader = new ModuleLoader(loggerFactory.CreateLogger<ModuleLoader>());

			// act
			var modules = loader.GetSortedModules("../../../../Host/bin/Debug/net6.0/", false);

			// assert
			Assert.AreEqual(0, loader.Errors.Count); // todo: fix errors in github actions tests
			Assert.IsTrue(modules.Count > 0);
		}

		[Test]
		[TestCase("Microsoft.Extensions.Http.dll", true)]
		[TestCase("System.Data.SqlClient.dll", true)]
		[TestCase("Montr.Core.dll", false)]
		public void ExcludeAssembly_ForFile_ReturnsExpected(string file, bool expected)
		{
			// arrange
			var loggerFactory = LoggerFactory.Create(
				builder => builder.AddConsole(options => options.FormatterName = "systemd"));
			var loader = new ModuleLoader(loggerFactory.CreateLogger<ModuleLoader>());

			// act
			var result = loader.ExcludeAssembly(file);

			// assert
			Assert.AreEqual(expected, result);
		}
	}
}

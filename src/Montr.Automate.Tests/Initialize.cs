using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Tests
{
	[TestClass]
	public class Initialize
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			DbConfigurationExtensions.SetLinq2DbTestSettings();
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
		}
	}
}

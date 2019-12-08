using LinqToDB.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Tests
{
	[TestClass]
	public class Initialize
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			IdentitySchemaMapper.MapSchema(MappingSchema.Default);

			DbConfigurationExtensions.SetLinq2DbTestSettings();
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
		}
	}
}

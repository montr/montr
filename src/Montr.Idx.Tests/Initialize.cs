using LinqToDB.Mapping;
using Montr.Core.Services.Implementations;
using Montr.Idx.Impl.Services;
using NUnit.Framework;

namespace Montr.Idx.Tests
{
	[SetUpFixture]
	public class Initialize
	{
		[OneTimeSetUp]
		public static void AssemblyInitialize()
		{
			IdentitySchemaMapper.MapSchema(MappingSchema.Default);

			DbConfigurationExtensions.SetLinq2DbTestSettings();
		}

		[OneTimeTearDown]
		public static void AssemblyCleanup()
		{
		}
	}
}

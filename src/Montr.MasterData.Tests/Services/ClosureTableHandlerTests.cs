using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Impl;
using Montr.MasterData.Models;
using NUnit.Framework;

namespace Montr.MasterData.Tests.Services
{
	[TestFixture]
	public class ClosureTableHandlerTests
	{
		[Test]
		public async Task InsertGroup_Should_BuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);

				// act
				await generator.InsertGroups(root.Uid,2, 3, null, null, cancellationToken);

				// assert
				var closure = generator.PrintClosure(root.Code);
				await AssertClosureEqual("../../../Content/closure.2x3.txt", closure, cancellationToken);
			}
		}

		[Test]
		public async Task UpdateGroup_ShouldThrow_WhenCyclicDependencyDetected()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await generator.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert - cyclic dependency
				var result = await generator.UpdateGroup(root.Code, "1.1", "1.1.1", cancellationToken, false);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.IsNotNull(result.Errors);
				Assert.AreEqual("parentUid", result.Errors[0].Key);
				Assert.AreEqual("Cyclic dependency detected.", result.Errors[0].Messages[0]);
			}
		}

		[Test]
		public async Task UpdateGroup_Should_RebuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await generator.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert - from null to not null parent
				await generator.UpdateGroup(root.Code, "1", "2.1", cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3~1to2.1.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert - from not null to null parent
				await generator.UpdateGroup(root.Code, "2.2", null, cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3~1to2.1~2.2toRoot.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert - from not null to not null parent
				await generator.UpdateGroup(root.Code, "3.3", "1.3", cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3~1to2.1~2.2toRoot~3.3to1.3.txt", generator.PrintClosure(root.Code), cancellationToken);
			}
		}

		[Test]
		public async Task DeleteGroup_Should_RebuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await generator.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert
				await generator.DeleteGroup(root.Code, "1", cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3-1.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert
				await generator.DeleteGroup(root.Code, "2.2", cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3-1-2.2.txt", generator.PrintClosure(root.Code), cancellationToken);

				// act & assert
				await generator.DeleteGroup(root.Code, "3.1.2", cancellationToken);
				await AssertClosureEqual("../../../Content/closure.3x3-1-2.2-3.1.2.txt", generator.PrintClosure(root.Code), cancellationToken);
			}
		}

		private async Task AssertClosureEqual(string expectedFileName, string actual, CancellationToken cancellationToken)
		{
			var expected = await File.ReadAllTextAsync(expectedFileName, cancellationToken);

			Assert.AreEqual(expected.Replace("\r\n", "\n"), actual.Replace("\r\n", "\n"), $"Invalid closure table, expected equal to content of {expectedFileName}");
		}
	}
}

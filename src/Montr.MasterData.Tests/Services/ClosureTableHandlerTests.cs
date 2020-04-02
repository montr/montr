using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class ClosureTableHandlerTests
	{
		[TestMethod]
		public async Task InsertGroup_Should_BuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();

			var generator = new DbHelper(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);

				// act
				await generator.InsertGroups(root.Uid,2, 3, null, null, cancellationToken);

				// assert
				var closure = generator.PrintClosure(root.Code);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.2x3.txt", cancellationToken), closure);
			}
		}

		[TestMethod]
		public async Task UpdateGroup_ShouldThrow_WhenCyclicDependencyDetected()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await dbHelper.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3.txt", cancellationToken), dbHelper.PrintClosure(root.Code));

				// act & assert - cyclic dependency
				var result = await dbHelper.UpdateGroup(root.Code, "1.1", "1.1.1", cancellationToken, false);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.IsNotNull(result.Errors);
				Assert.AreEqual("parentUid", result.Errors[0].Key);
				Assert.AreEqual("Cyclic dependency detected.", result.Errors[0].Messages[0]);
			}
		}

		[TestMethod]
		public async Task UpdateGroup_Should_RebuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new DbHelper(unitOfWorkFactory, dbContextFactory);
			
			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await generator.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3.txt", cancellationToken), generator.PrintClosure(root.Code));

				// act & assert - from null to not null parent
				await generator.UpdateGroup(root.Code, "1", "2.1", cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3~1to2.1.txt", cancellationToken), generator.PrintClosure(root.Code));

				// act & assert - from not null to null parent
				await generator.UpdateGroup(root.Code, "2.2", null, cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3~1to2.1~2.2toRoot.txt", cancellationToken), generator.PrintClosure(root.Code));

				// act & assert - from not null to not null parent
				await generator.UpdateGroup(root.Code, "3.3", "1.3", cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3~1to2.1~2.2toRoot~3.3to1.3.txt", cancellationToken), generator.PrintClosure(root.Code));
			}
		}

		[TestMethod]
		public async Task DeleteGroup_Should_RebuildClosureTable()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				await dbHelper.InsertGroups(root.Uid, 3, 3, null, null, cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3.txt", cancellationToken), dbHelper.PrintClosure(root.Code));

				// act & assert
				await dbHelper.DeleteGroup(root.Code, "1", cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3-1.txt", cancellationToken), dbHelper.PrintClosure(root.Code));

				// act & assert
				await dbHelper.DeleteGroup(root.Code, "2.2", cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3-1-2.2.txt", cancellationToken), dbHelper.PrintClosure(root.Code));

				// act & assert
				await dbHelper.DeleteGroup(root.Code, "3.1.2", cancellationToken);
				Assert.AreEqual(await File.ReadAllTextAsync("../../../Content/closure.3x3-1-2.2-3.1.2.txt", cancellationToken), dbHelper.PrintClosure(root.Code));
			}
		}
	}
}

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
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
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);

				// act
				await generator.InsertGroups(2, 3, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);

				// assert
				var closure = generator.PrintClosure();
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.2x3.txt"), closure);
			}
		}

		[TestMethod]
		public async Task UpdateGroup_ShouldThrow_WhenCyclicDependencyDetected()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);
				await generator.InsertGroups(3, 3, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), generator.PrintClosure());

				// act & assert - cyclic dependency
				var result = await generator.UpdateGroup(root.Code + ".1.1", root.Code + ".1.1.1", updateClassifierGroupHandler, cancellationToken);
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
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var updateClassifierGroupHandler = new UpdateClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);
				await generator.InsertGroups(3, 3, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), generator.PrintClosure());

				// act & assert - from null to not null parent
				await generator.UpdateGroup(root.Code + ".1", root.Code + ".2.1", updateClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1.txt"), generator.PrintClosure());

				// act & assert - from not null to null parent
				await generator.UpdateGroup(root.Code + ".2.2", root.Code, updateClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1~2.2toRoot.txt"), generator.PrintClosure());

				// act & assert - from not null to not null parent
				await generator.UpdateGroup(root.Code + ".3.3", root.Code + ".1.3", updateClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3~1to2.1~2.2toRoot~3.3to1.3.txt"), generator.PrintClosure());
			}
		}

		[TestMethod]
		public async Task DeleteGroup_Should_RebuildClosureTable()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var deleteClassifierGroupHandler = new DeleteClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			using (var _ = unitOfWorkFactory.Create())
			{
				// act & assert
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);
				await generator.InsertGroups(3, 3, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3.txt"), generator.PrintClosure());

				// act & assert
				await generator.DeleteGroup(root.Code + ".1", deleteClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1.txt"), generator.PrintClosure());

				// act & assert
				await generator.DeleteGroup(root.Code + ".2.2", deleteClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2.txt"), generator.PrintClosure());

				// act & assert
				await generator.DeleteGroup(root.Code + ".3.1.2", deleteClassifierGroupHandler, cancellationToken);
				Assert.AreEqual(File.ReadAllText("../../../Content/closure.3x3-1-2.2-3.1.2.txt"), generator.PrintClosure());
			}
		}
	}
}

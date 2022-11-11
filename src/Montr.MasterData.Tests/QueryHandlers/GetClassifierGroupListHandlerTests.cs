using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Services.QueryHandlers;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestFixture]
	public class GetClassifierGroupListHandlerTests
	{
		[Test]
		public async Task GetGroups_ForNullParent_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);

				var group1 = await generator.InsertGroup(tree.Uid, "001", null, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.001", group1.Uid, cancellationToken);

				var group2 = await generator.InsertGroup(tree.Uid, "002", null, cancellationToken);
				var group3 = await generator.InsertGroup(tree.Uid, "003", null, cancellationToken);
				var group4 = await generator.InsertGroup(tree.Uid, "004", null, cancellationToken);

				// act
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					TreeUid = tree.Uid,
					ParentUid = null // yeah, we test this
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(4, result.Rows.Count);
				Assert.AreEqual(group1.Uid, result.Rows[0].Uid);
				Assert.AreEqual(group2.Uid, result.Rows[1].Uid);
				Assert.AreEqual(group3.Uid, result.Rows[2].Uid);
				Assert.AreEqual(group4.Uid, result.Rows[3].Uid);
			}
		}

		[Test]
		public async Task GetGroups_ForNotNullParent_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);

				var parentGroup = await generator.InsertGroup(tree.Uid, "001", null, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.001", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.002", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.003", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.004", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.005", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.006", parentGroup.Uid, cancellationToken);
				await generator.InsertGroup(tree.Uid, "001.007", parentGroup.Uid, cancellationToken);

				// act
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					TreeUid = tree.Uid,
					ParentUid = parentGroup.Uid,
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(7, result.Rows.Count);
			}
		}

		[Test, Ignore("implement")]
		public async Task GetGroups_WhenAutoExpandSingleChildRequested_ShouldReturnExpanded()
		{
			await Task.FromException<NotImplementedException>(new NotImplementedException());
		}

		[Test]
		public async Task GetItems_ForNullParent_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Items, cancellationToken);
				for (var i = 0; i < 21; i++)
				{
					await generator.InsertItem($"{i:D4}", null, cancellationToken);
				}

				// act
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					PageSize = 100
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(21, result.Rows.Count);
			}
		}

		[Test]
		public async Task GetItems_ForNotNullParent_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Items, cancellationToken);
				var parentItem = await generator.InsertItem("001", null, cancellationToken);
				for (var i = 0; i < 3; i++)
				{
					await generator.InsertItem($"{i:D4}", parentItem.Uid, cancellationToken);
				}

				// act
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					ParentUid = parentItem.Uid
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(3, result.Rows.Count);
			}
		}

		[Test, Ignore("implement")]
		public async Task GetItems_WhenAutoExpandSingleChildRequested_ShouldReturnExpanded()
		{
			await Task.FromException<NotImplementedException>(new NotImplementedException());
		}

		[Test, Ignore("todo: check this - what should be for a lot of groups")]
		public async Task GetItems_ForBigGroups_ReturnNoMoreThan1000Items()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				// todo: generate a lot of groups

				// act
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					TreeUid = tree.Uid
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Rows.Count <= 1000);
			}
		}

		[Test]
		public async Task GetGroups_WithFocusUid_ReturnChildrenOfEachParentGroups()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var classifierTreeService = new DefaultClassifierTreeService(classifierTreeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierGroupListHandler(dbContextFactory, classifierTypeService, classifierTreeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);

				// 2 roots without parent
				for (var i = 0; i < 2; i++)
				{
					await generator.InsertGroup(tree.Uid, $"1-{i:D4}", null, cancellationToken);
				}

				// 1 root with 4 deep children
				Guid? parentUid = null;
				for (var i = 0; i <= 5; i++)
				{
					var childGroup = await generator.InsertGroup(tree.Uid, $"2-{i:D4}", parentUid, cancellationToken);
					parentUid = childGroup.Uid;
				}

				// 3 roots without parent
				for (var i = 0; i < 3; i++)
				{
					await generator.InsertGroup(tree.Uid, $"3-{i:D4}", null, cancellationToken);
				}

				var focusUid = parentUid;

				// act & assert - focus in root scope
				var command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					TreeUid = tree.Uid,
					ParentUid = null,
					FocusUid = focusUid
					// PageSize = 100 // todo: common limit for all levels is not ok
				};

				var result = await handler.Handle(command, cancellationToken);

				// todo: pretty print and compare by focus.txt
				Assert.IsNotNull(result);
				Assert.AreEqual(6, result.Rows.Count);
				Assert.IsNull(result.Rows[0].Children);
				Assert.IsNull(result.Rows[1].Children);
				Assert.IsNull(result.Rows[3].Children);
				Assert.IsNull(result.Rows[4].Children);
				Assert.IsNull(result.Rows[5].Children);

				Assert.IsNotNull(result.Rows[2].Children);
				Assert.AreEqual(result.Rows[2].Children.Count, 1); // 0 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children.Count, 1);  // 1 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children.Count, 1);  // 2 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children[0].Children.Count, 1);  // 3 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children[0].Children[0].Children.Count, 1);  // 4 lvl
				Assert.IsNull(result.Rows[2].Children[0].Children[0].Children[0].Children[0].Children[0].Children);

				// act & assert - focus in scope of selected parent
				// todo: what is the difference with previous asserts - ParentUid should be set?
				command = new GetClassifierGroupList
				{
					TypeCode = generator.TypeCode,
					TreeUid = tree.Uid,
					FocusUid = focusUid
				};

				result = await handler.Handle(command, cancellationToken);

				// todo: pretty print and compare by focus.txt
				Assert.IsNotNull(result);
				Assert.AreEqual(6, result.Rows.Count);
				Assert.IsNull(result.Rows[0].Children);
				Assert.IsNull(result.Rows[1].Children);
				Assert.IsNull(result.Rows[3].Children);
				Assert.IsNull(result.Rows[4].Children);
				Assert.IsNull(result.Rows[5].Children);

				Assert.IsNotNull(result.Rows[2].Children);
				Assert.AreEqual(result.Rows[2].Children.Count, 1); // 0 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children.Count, 1);  // 1 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children.Count, 1);  // 2 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children[0].Children.Count, 1);  // 3 lvl
				Assert.AreEqual(result.Rows[2].Children[0].Children[0].Children[0].Children[0].Children.Count, 1);  // 4 lvl
				Assert.IsNull(result.Rows[2].Children[0].Children[0].Children[0].Children[0].Children[0].Children);
			}
		}
	}
}

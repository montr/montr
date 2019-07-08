using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class DeleteClassifierLinkHandlerTests
	{
		[TestMethod]
		public async Task Handle_InSecondaryHierarchy_ShouldDeleteExistingLink()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var root2 = await dbHelper.InsertTree("root2", cancellationToken);
				var group2 = await dbHelper.InsertGroup("002", root2.Uid, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", cancellationToken);
				await dbHelper.InsertLink(group2.Uid, item1.Uid, cancellationToken);

				// assert - links to default and secondary hierarchies exists
				var links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(2, links.TotalCount);
				var groups = links.Rows.Select(x => x.Group.Uid).ToList();
				CollectionAssert.Contains(groups, root.Uid);
				CollectionAssert.Contains(groups, group2.Uid);

				// act
				var result = await handler.Handle(new DeleteClassifierLink
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - link deleted
				Assert.IsTrue(result.Success);

				// assert - link to default hierarchy still exists
				links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(root.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);
			}
		}

		[TestMethod]
		public async Task Handle_LastLinkInDefaultHierarchy_ShouldThrow()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await dbHelper.InsertGroup("001", root.Uid, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", cancellationToken);
				await dbHelper.InsertLink(group1.Uid, item1.Uid, cancellationToken);

				// assert - links in default hierarchy exists
				var links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group1.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);

				// act
				var result = await handler.Handle(new DeleteClassifierLink
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group1.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - link not deleted
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.Errors.Count);

				// assert - link to default hierarchy root exists
				links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(root.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);
			}
		}
	}
}

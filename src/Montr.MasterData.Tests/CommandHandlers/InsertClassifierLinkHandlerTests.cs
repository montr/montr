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
	public class InsertClassifierLinkHandlerTests
	{
		[TestMethod]
		public async Task Handle_WithExistingLink_ShouldDeleteExistingLinkAndInsertNewLink()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await dbHelper.InsertGroup("001", root.Uid, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", cancellationToken);

				// assert - initially new items belongs to default group
				var links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(root.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group1.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link is inserted
				Assert.IsTrue(result.Success);

				// assert - item linked to new group, link with root not exists
				links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group1.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);
			}
		}

		[TestMethod]
		public async Task Handle_WithoutExistingLink_ShouldInsertNewLink()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var root2 = await dbHelper.InsertTree("root2", cancellationToken);
				var group2 = await dbHelper.InsertGroup("002", root2.Uid, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", cancellationToken);

				// assert - initially new items belongs to default group
				var links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(root.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link inserted
				Assert.IsTrue(result.Success);

				// assert - item linked to new group, link with default root still exists
				links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(2, links.TotalCount);
				var groups = links.Rows.Select(x => x.Group.Uid).ToList();
				CollectionAssert.Contains(groups, root.Uid);
				CollectionAssert.Contains(groups, group2.Uid);
			}
		}
	}
}

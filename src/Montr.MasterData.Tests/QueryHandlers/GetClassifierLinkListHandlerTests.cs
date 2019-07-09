using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierLinkListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierLinkList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierLinkListHandler(dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await dbHelper.InsertGroup(root.Uid, "001", null, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", cancellationToken);
				var item2 = await dbHelper.InsertItem("002", cancellationToken);
				await dbHelper.InsertLink(group1.Uid, item1.Uid, cancellationToken);
				await dbHelper.InsertLink(group1.Uid, item2.Uid, cancellationToken);

				// act - search by group code
				var result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = dbHelper.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = dbHelper.CompanyUid,
						TypeCode = dbHelper.TypeCode,
						GroupUid = group1.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.TotalCount);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(group1.Uid, result.Rows[1].Group.Uid);
				var items = result.Rows.Select(x => x.Item.Uid).ToList();
				CollectionAssert.Contains(items, item1.Uid);
				CollectionAssert.Contains(items, item2.Uid);

				// act - search by item code
				result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = dbHelper.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = dbHelper.CompanyUid,
						TypeCode = dbHelper.TypeCode,
						ItemUid = item1.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, result.Rows[0].Item.Uid);

				// act - search by both group and item codes
				result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = dbHelper.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = dbHelper.CompanyUid,
						TypeCode = dbHelper.TypeCode,
						GroupUid = group1.Uid,
						ItemUid = item2.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(item2.Uid, result.Rows[0].Item.Uid);
			}
		}
	}
}

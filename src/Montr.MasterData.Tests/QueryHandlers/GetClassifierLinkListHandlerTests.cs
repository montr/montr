using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestFixture]
	public class GetClassifierLinkListHandlerTests
	{
		[Test]
		public async Task GetClassifierLinkList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierLinkListHandler(dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await generator.InsertGroup(tree.Uid, "001", null, cancellationToken);
				var item1 = await generator.InsertItem("001", null, cancellationToken);
				var item2 = await generator.InsertItem("002", null, cancellationToken);
				await generator.InsertLink(group1.Uid, item1.Uid, cancellationToken);
				await generator.InsertLink(group1.Uid, item2.Uid, cancellationToken);

				// act - search by group code
				var result = await handler.Handle(new GetClassifierLinkList
				{
					TypeCode = generator.TypeCode,
					GroupUid = group1.Uid
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.TotalCount);
				Assert.AreEqual(tree.Uid, result.Rows[0].Tree.Uid);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(tree.Uid, result.Rows[1].Tree.Uid);
				Assert.AreEqual(group1.Uid, result.Rows[1].Group.Uid);
				var items = result.Rows.Select(x => x.Item.Uid).ToList();
				CollectionAssert.Contains(items, item1.Uid);
				CollectionAssert.Contains(items, item2.Uid);

				// act - search by item code
				result = await handler.Handle(new GetClassifierLinkList
				{
					TypeCode = generator.TypeCode,
					ItemUid = item1.Uid
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(tree.Uid, result.Rows[0].Tree.Uid);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, result.Rows[0].Item.Uid);

				// act - search by both group and item codes
				result = await handler.Handle(new GetClassifierLinkList
				{
					TypeCode = generator.TypeCode,
					GroupUid = group1.Uid,
					ItemUid = item2.Uid
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(tree.Uid, result.Rows[0].Tree.Uid);
				Assert.AreEqual(group1.Uid, result.Rows[0].Group.Uid);
				Assert.AreEqual(item2.Uid, result.Rows[0].Item.Uid);
			}
		}
	}
}

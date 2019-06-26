using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.CommandHandlers;
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
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();

			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			// var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var insertClassifierHandler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService);

			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);
			var cancellationToken = new CancellationToken();

			var handler = new GetClassifierLinkListHandler(dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);
				// await generator.InsertGroups(3, 1, root.Code, root.Uid, insertClassifierGroupHandler, cancellationToken);
				var item1 = await generator.InsertItem("001", insertClassifierHandler, cancellationToken);
				var item2 = await generator.InsertItem("002", insertClassifierHandler, cancellationToken);

				// act - search by group code
				var result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = generator.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = generator.CompanyUid,
						TypeCode = generator.TypeCode,
						GroupUid = root.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(2, result.TotalCount);
				Assert.AreEqual(root.Uid, result.Rows[0].GroupUid);
				Assert.AreEqual(root.Uid, result.Rows[1].GroupUid);
				var items = result.Rows.Select(x => x.ItemUid).ToList();
				CollectionAssert.Contains(items, item1.Uid);
				CollectionAssert.Contains(items, item2.Uid);

				// act - search by item code
				result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = generator.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = generator.CompanyUid,
						TypeCode = generator.TypeCode,
						ItemUid = item1.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(root.Uid, result.Rows[0].GroupUid);
				Assert.AreEqual(item1.Uid, result.Rows[0].ItemUid);

				// act - search by both group and item codes
				result = await handler.Handle(new GetClassifierLinkList
				{
					UserUid = generator.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = generator.CompanyUid,
						TypeCode = generator.TypeCode,
						GroupUid = root.Uid,
						ItemUid = item2.Uid
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
				Assert.AreEqual(root.Uid, result.Rows[0].GroupUid);
				Assert.AreEqual(item2.Uid, result.Rows[0].ItemUid);
			}
		}
	}
}

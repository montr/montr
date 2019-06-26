using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class InsertClassifierLinkHandlerTests
	{
		[TestMethod]
		public async Task InsertLink_Should_InsertLink()
		{
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var dateTimeProvider = new DefaultDateTimeProvider();
			var insertClassifierGroupHandler = new InsertClassifierGroupHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);
			var insertClassifierHandler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService);
			var getClassifierLinkListHandler = new GetClassifierLinkListHandler(dbContextFactory, classifierTypeService);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			var handler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindGroup(ClassifierGroup.DefaultRootCode, cancellationToken);
				var group1 = await generator.InsertGroup("001", root.Uid, insertClassifierGroupHandler, cancellationToken);
				var item1 = await generator.InsertItem("001", insertClassifierHandler,  cancellationToken);

				// assert - initially new items belongs to default group
				var links = await getClassifierLinkListHandler.Handle(new GetClassifierLinkList
				{
					UserUid = generator.UserUid,
                    Request = new ClassifierLinkSearchRequest
                    {
                        CompanyUid = generator.CompanyUid,
                        TypeCode = generator.TypeCode,
                        ItemUid = item1.Uid
                    }
				}, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(root.Uid, links.Rows[0].GroupUid);
				Assert.AreEqual(item1.Uid, links.Rows[0].ItemUid);

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					Item = new ClassifierLink
					{
						GroupUid = group1.Uid,
						ItemUid = item1.Uid
					}
				}, cancellationToken);

				// assert - new link is inserted
				Assert.IsTrue(result.Success);

				// assert - item linked to new group, link with root not exists
				links = await getClassifierLinkListHandler.Handle(new GetClassifierLinkList
				{
					UserUid = generator.UserUid,
					Request = new ClassifierLinkSearchRequest
					{
						CompanyUid = generator.CompanyUid,
                        TypeCode = generator.TypeCode,
						ItemUid = item1.Uid
					}
				}, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group1.Uid, links.Rows[0].GroupUid);
				Assert.AreEqual(item1.Uid, links.Rows[0].ItemUid);

                // todo: add test that other hierarchies links are not deleted
			}
		}
	}
}

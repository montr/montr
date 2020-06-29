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
using Montr.MasterData.Tests.Services;

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
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await generator.InsertGroup(root.Uid, "001", null, cancellationToken);
				var group2 = await generator.InsertGroup(root.Uid, "002", null, cancellationToken);
				var item1 = await generator.InsertItem("001", null, cancellationToken);
				await generator.InsertLink(group1.Uid, item1.Uid, cancellationToken);

				// assert - initially new items belongs to default group
				var links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group1.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link is inserted
				Assert.IsTrue(result.Success);

				// assert - item linked to new group, link with root not exists
				links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group2.Uid, links.Rows[0].Group.Uid);
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
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await generator.InsertGroup(root.Uid, "001", null, cancellationToken);
				var root2 = await generator.InsertTree("root2", cancellationToken);
				// ReSharper disable once PossibleInvalidOperationException
				var group2 = await generator.InsertGroup(root2.Uid.Value, "002", null, cancellationToken);
				var item1 = await generator.InsertItem("001", null, cancellationToken);
				await generator.InsertLink(group1.Uid, item1.Uid, cancellationToken);

				// assert - initially new items belongs to default group
				var links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(1, links.TotalCount);
				Assert.AreEqual(group1.Uid, links.Rows[0].Group.Uid);
				Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link inserted
				Assert.IsTrue(result.Success);

				// assert - item linked to new group, link with default root still exists
				links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.AreEqual(2, links.TotalCount);
				var groups = links.Rows.Select(x => x.Group.Uid).ToList();
				CollectionAssert.Contains(groups, group1.Uid);
				CollectionAssert.Contains(groups, group2.Uid);
			}
		}
	}
}

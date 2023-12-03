using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services.CommandHandlers;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestFixture]
	public class DeleteClassifierLinkHandlerTests
	{
		[Test]
		public async Task Handle_InSecondaryHierarchy_ShouldDeleteExistingLink()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbHelper = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await dbHelper.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var root2 = await dbHelper.InsertTree("root2", cancellationToken);
				// ReSharper disable once PossibleInvalidOperationException
				var group2 = await dbHelper.InsertGroup(root2.Uid.Value, "002", null, cancellationToken);
				var item1 = await dbHelper.InsertItem("001", null, cancellationToken);
				await dbHelper.InsertLink(group2.Uid, item1.Uid, cancellationToken);

				// assert - links to --default and-- secondary hierarchy exists
				var links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(1));
				var groups = links.Rows.Select(x => x.Group.Uid).ToList();
				// CollectionAssert.Contains(groups, root.Uid);
				Assert.That(groups, Has.Member(group2.Uid));

				// act
				var result = await handler.Handle(new DeleteClassifierLink
				{
					UserUid = dbHelper.UserUid,
					TypeCode = dbHelper.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - link deleted
				Assert.That(result.Success);

				// assert - NO link to default hierarchy exists
				links = await dbHelper.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(0));
				// Assert.AreEqual(root.Uid, links.Rows[0].Group.Uid);
				// Assert.AreEqual(item1.Uid, links.Rows[0].Item.Uid);
			}
		}

		[Test, Ignore("No links to default root, restore test later")]
		public async Task Handle_LastLinkInDefaultHierarchy_ShouldThrow()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierLinkHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var root = await generator.FindTree(ClassifierTree.DefaultCode, cancellationToken);
				var group1 = await generator.InsertGroup(root.Uid, "001", null, cancellationToken);
				var item1 = await generator.InsertItem("001", null, cancellationToken);
				await generator.InsertLink(group1.Uid, item1.Uid, cancellationToken);

				// assert - links in default hierarchy exists
				var links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(1));
				Assert.That(links.Rows[0].Group.Uid, Is.EqualTo(group1.Uid));
				Assert.That(links.Rows[0].Item.Uid, Is.EqualTo(item1.Uid));

				// act
				var result = await handler.Handle(new DeleteClassifierLink
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group1.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - link not deleted
				Assert.That(result.Success);
				Assert.That(result.Errors.Count, Is.EqualTo(1));

				// assert - link to default hierarchy root exists
				links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(1));
				Assert.That(links.Rows[0].Group.Uid, Is.EqualTo(root.Uid));
				Assert.That(links.Rows[0].Item.Uid, Is.EqualTo(item1.Uid));
			}
		}
	}
}

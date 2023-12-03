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
	public class InsertClassifierLinkHandlerTests
	{
		[Test]
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

				Assert.That(links.TotalCount, Is.EqualTo(1));
				Assert.That(links.Rows[0].Group.Uid, Is.EqualTo(group1.Uid));
				Assert.That(links.Rows[0].Item.Uid, Is.EqualTo(item1.Uid));

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link is inserted
				Assert.That(result.Success);

				// assert - item linked to new group, link with root not exists
				links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(1));
				Assert.That(links.Rows[0].Group.Uid, Is.EqualTo(group2.Uid));
				Assert.That(links.Rows[0].Item.Uid, Is.EqualTo(item1.Uid));
			}
		}

		[Test]
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

				Assert.That(links.TotalCount, Is.EqualTo(1));
				Assert.That(links.Rows[0].Group.Uid, Is.EqualTo(group1.Uid));
				Assert.That(links.Rows[0].Item.Uid, Is.EqualTo(item1.Uid));

				// act - link with new group in same hierarchy
				var result = await handler.Handle(new InsertClassifierLink
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					GroupUid = group2.Uid.Value,
					// ReSharper disable once PossibleInvalidOperationException
					ItemUid = item1.Uid.Value
				}, cancellationToken);

				// assert - new link inserted
				Assert.That(result.Success);

				// assert - item linked to new group, link with default root still exists
				links = await generator.GetLinks(null, item1.Uid, cancellationToken);

				Assert.That(links.TotalCount, Is.EqualTo(2));
				var groups = links.Rows.Select(x => x.Group.Uid).ToList();
				Assert.That(groups, Has.Member(group1.Uid));
				Assert.That(groups, Has.Member(group2.Uid));
			}
		}
	}
}

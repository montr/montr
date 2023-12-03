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
	public class DeleteClassifierTreeHandlerTests
	{
		[Test]
		public async Task Handle_NormalValues_ShouldDeleteTree()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierTreeHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var tree2 = await generator.InsertTree("tree2", cancellationToken);

				// assert - default and second trees exists
				var trees = await generator.GetTrees(cancellationToken);
				Assert.That(trees.TotalCount, Is.EqualTo(2));

				// act
				var result = await handler.Handle(new DeleteClassifierTree
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					Uids = new [] { tree2.Uid.Value }
				}, cancellationToken);

				// assert - link deleted
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(1));

				// assert - default hierarchy exists
				trees = await generator.GetTrees(cancellationToken);
				Assert.That(trees.TotalCount, Is.EqualTo(1));
				Assert.That(trees.Rows[0].Code, Is.EqualTo(ClassifierTree.DefaultCode));
			}
		}
	}

	// todo: test - should not delete default tree
}

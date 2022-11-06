using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Impl;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
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
				Assert.AreEqual(2, trees.TotalCount);

				// act
				var result = await handler.Handle(new DeleteClassifierTree
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					Uids = new [] { tree2.Uid.Value }
				}, cancellationToken);

				// assert - link deleted
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				// assert - default hierarchy exists
				trees = await generator.GetTrees(cancellationToken);
				Assert.AreEqual(1, trees.TotalCount);
				Assert.AreEqual(ClassifierTree.DefaultCode, trees.Rows[0].Code);
			}
		}
	}

	// todo: test - should not delete default tree
}

using System;
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
	public class InsertClassifierTreeHandlerTests
	{
		[Test]
		public async Task Handle_NormalValues_InsertClassifierTree()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierTreeHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);

				// act
				var result = await handler.Handle(new InsertClassifierTree
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Item = new ClassifierTree
					{
						Code = "001",
						Name = "Test Classifier Tree"
					}
				}, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.Uid, Is.Not.Null);
				Assert.That(result.Uid, Is.Not.EqualTo(Guid.Empty));

				var trees = await generator.GetTrees(cancellationToken);
				Assert.That(trees.TotalCount, Is.EqualTo(2)); // one for default tree and second for inserted in test
			}
		}

		// todo: duplicate code error test
		// todo: invalid hierarchy type test (?)
	}
}

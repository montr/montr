using System;
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
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);

				var trees = await generator.GetTrees(cancellationToken);
				Assert.AreEqual(2, trees.TotalCount); // one for default tree and second for inserted in test
			}
		}

		// todo: duplicate code error test
		// todo: invalid hierarchy type test (?)
	}
}

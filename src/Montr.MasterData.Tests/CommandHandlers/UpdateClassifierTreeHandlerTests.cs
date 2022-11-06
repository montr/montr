using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestFixture]
	public class UpdateClassifierTreeHandlerTests
	{
		[Test]
		public async Task Handle_ByUid_UpdateClassifierTree()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new UpdateClassifierTreeHandler(unitOfWorkFactory, dbContextFactory, classifierTypeService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.Groups, cancellationToken);
				var trees = await generator.GetTrees(cancellationToken);

				// act
				var result = await handler.Handle(new UpdateClassifierTree
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Item = new ClassifierTree
					{
						Uid = trees.Rows[0].Uid,
						Code = ClassifierTree.DefaultCode,
						Name = "Test Classifier Tree"
					}
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				trees = await generator.GetTrees(cancellationToken);
				Assert.AreEqual(1, trees.TotalCount);
				Assert.AreEqual("Test Classifier Tree", trees.Rows[0].Name);
			}
		}

		// todo: do not update default tree code (?)
	}
}

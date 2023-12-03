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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(1));

				trees = await generator.GetTrees(cancellationToken);
				Assert.That(trees.TotalCount, Is.EqualTo(1));
				Assert.That(trees.Rows[0].Name, Is.EqualTo("Test Classifier Tree"));
			}
		}

		// todo: do not update default tree code (?)
	}
}

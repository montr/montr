using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Services.QueryHandlers;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestFixture]
	public class GetClassifierTreeListHandlerTests
	{
		[Test]
		public async Task GetClassifierTreeList_ReturnList()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTreeRepository = new DbClassifierTreeRepository(dbContextFactory);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierTreeListHandler(classifierTreeRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// default tree will be insered for HierarchyType.Groups
				await generator.InsertType(HierarchyType.Groups, cancellationToken);

				// act
				var result = await handler.Handle(new GetClassifierTreeList
				{
					TypeCode = generator.TypeCode
				}, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.TotalCount);
			}
		}
	}
}

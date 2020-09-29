using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.QueryHandlers;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Montr.Core.Tests.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestClass]
	public class GetEntityStatusListHandlerTests
	{
		[TestMethod]
		public async Task GetEntityStatusList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var entityStatusRepository = new DbEntityStatusRepository(dbContextFactory);
			var generator = new CoreDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetEntityStatusListHandler(entityStatusRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				for (var i = 0; i < 42; i++)
				{
					await generator.InsertEntityStatus(new EntityStatus
					{
						Code = "status_code_" + i,
						Name = "Status Name " + i
					}, cancellationToken);
				}

				// act
				var request = new GetEntityStatusList
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityUid
				};
				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(10, result.Rows.Count);
				Assert.AreEqual(42, result.TotalCount);
			}
		}
	}
}

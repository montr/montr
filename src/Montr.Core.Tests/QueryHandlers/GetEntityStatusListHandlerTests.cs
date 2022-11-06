using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.QueryHandlers;
using Montr.Core.Services.Impl;
using Montr.Core.Tests.Services;
using NUnit.Framework;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestFixture]
	public class GetEntityStatusListHandlerTests
	{
		[Test]
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

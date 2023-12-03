using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services.Implementations;
using Montr.Core.Services.QueryHandlers;
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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Rows.Count, Is.EqualTo(10));
				Assert.That(result.TotalCount, Is.EqualTo(42));
			}
		}
	}
}

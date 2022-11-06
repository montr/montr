using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Tests.Services;
using Moq;
using NUnit.Framework;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestFixture]
	public class GetNumeratorEntityListHandlerTests
	{
		[Test]
		public async Task GetNumeratorEntityList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var entityTypeResolverMock = new Mock<IEntityNameResolver>();
			entityTypeResolverMock.Setup(x => x.Resolve(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => "Numerator Entity Name");

			var entityTypeResolverFactory = new Mock<INamedServiceFactory<IEntityNameResolver>>();
			entityTypeResolverFactory.Setup(x => x.GetRequiredService(It.IsAny<string>()))
				.Returns(() => entityTypeResolverMock.Object);

			var repository = new DbNumeratorEntityRepository(dbContextFactory, entityTypeResolverFactory.Object);
			var handler = new GetNumeratorEntityListHandler(repository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var numerator = await generator.InsertNumerator(
					new Numerator(),
					new GenerateNumberRequest
					{
						EntityTypeCode = "NumeratorTypeCode",
						EntityTypeUid = Guid.NewGuid()
					}, cancellationToken);

				// act
				// ReSharper disable once PossibleInvalidOperationException
				var command = new GetNumeratorEntityList { NumeratorUid = numerator.Uid.Value };
				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsNull(result.TotalCount);
				Assert.IsTrue(result.Rows.Count > 0);
				Assert.IsTrue(result.Rows.Count(x => x.EntityTypeCode == "NumeratorTypeCode") == 1);
			}
		}
	}
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services.CommandHandlers;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Services.QueryHandlers;
using Montr.MasterData.Tests.Services;
using Moq;
using NUnit.Framework;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestFixture]
	public class SaveNumeratorEntityHandlerTests
	{
		[Test]
		public async Task Save_SelectedNumerator_ShouldSave()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var entityTypeResolverMock = new Mock<IEntityNameResolver>();
			entityTypeResolverMock.Setup(x => x.Resolve(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => "Numerator Entity Name");

			var entityTypeResolverFactory = new Mock<INamedServiceFactory<IEntityNameResolver>>();
			entityTypeResolverFactory.Setup(x => x.GetRequiredService(It.IsAny<string>()))
				.Returns(() => entityTypeResolverMock.Object);

			var repository = new DbNumeratorEntityRepository(dbContextFactory, entityTypeResolverFactory.Object);

			var handler = new SaveNumeratorEntityHandler(unitOfWorkFactory, dbContextFactory, repository);

			var getNumeratorEntityHandler = new GetNumeratorEntityHandler(repository);
			var getNumeratorEntityListHandler = new GetNumeratorEntityListHandler(repository);

			var entity1 = Guid.NewGuid();
			var entity2 = Guid.NewGuid();
			var entity3 = Guid.NewGuid();
			var entity4 = Guid.NewGuid();

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var numerator = await generator.InsertNumerator(
					new Numerator(),
					new GenerateNumberRequest { EntityTypeCode = "NumeratorTypeCode" }, cancellationToken);

				// act
				var result1 = await handler.Handle(new SaveNumeratorEntity
				{
					Item = new NumeratorEntity { NumeratorUid = numerator.Uid, EntityUid = entity1, IsAutoNumbering = true }
				}, cancellationToken);

				var result2 = await handler.Handle(new SaveNumeratorEntity
				{
					Item = new NumeratorEntity { NumeratorUid = numerator.Uid, EntityUid = entity2, IsAutoNumbering = false }
				}, cancellationToken);

				var result3 = await handler.Handle(new SaveNumeratorEntity
				{
					Item = new NumeratorEntity { NumeratorUid = null /* without numerator */, EntityUid = entity3, IsAutoNumbering = true }
				}, cancellationToken);

				var result4 = await handler.Handle(new SaveNumeratorEntity
				{
					Item = new NumeratorEntity { NumeratorUid = null /* without numerator */, EntityUid = entity4, IsAutoNumbering = false }
				}, cancellationToken);

				// assert
				Assert.That(result1.Success);
				Assert.That(result2.Success);
				Assert.That(result3.Success);
				Assert.That(result4.Success);

				var numerator1 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity1 }, cancellationToken);
				var numerator2 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity2 }, cancellationToken);
				var numerator3 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity3 }, cancellationToken);
				var numerator4 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity4 }, cancellationToken);

				Assert.That(numerator1.EntityUid, Is.EqualTo(entity1));
				Assert.That(numerator1.NumeratorUid, Is.EqualTo(numerator.Uid));
				Assert.That(numerator1.IsAutoNumbering, Is.EqualTo(true));

				Assert.That(numerator2.EntityUid, Is.EqualTo(entity2));
				Assert.That(numerator2.NumeratorUid, Is.EqualTo(numerator.Uid));
				Assert.That(numerator2.IsAutoNumbering, Is.EqualTo(false));

				Assert.That(numerator3.EntityUid, Is.EqualTo(entity3));
				Assert.That(numerator3.NumeratorUid, Is.EqualTo(null));
				Assert.That(numerator3.IsAutoNumbering, Is.EqualTo(true));

				Assert.That(numerator4.EntityUid, Is.EqualTo(entity4));
				Assert.That(numerator4.NumeratorUid, Is.EqualTo(null));
				Assert.That(numerator4.IsAutoNumbering, Is.EqualTo(false));

				var entityList = await getNumeratorEntityListHandler.Handle(new GetNumeratorEntityList { NumeratorUid = numerator.Uid }, cancellationToken);
				Assert.That(entityList.Rows, Has.Count.EqualTo(2));
				Assert.That(entityList.Rows.Count(x => x.IsAutoNumbering), Is.EqualTo(1));
				Assert.That(entityList.Rows.Count(x => x.IsAutoNumbering == false), Is.EqualTo(1));
			}
		}
	}
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Tests.Services;
using Moq;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class SaveNumeratorEntityHandlerTests
	{
		[TestMethod]
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
				Assert.IsTrue(result1.Success);
				Assert.IsTrue(result2.Success);
				Assert.IsTrue(result3.Success);
				Assert.IsTrue(result4.Success);

				var numerator1 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity1 }, cancellationToken);
				var numerator2 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity2 }, cancellationToken);
				var numerator3 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity3 }, cancellationToken);
				var numerator4 = await getNumeratorEntityHandler.Handle(new GetNumeratorEntity { EntityUid = entity4 }, cancellationToken);

				Assert.AreEqual(entity1, numerator1.EntityUid);
				Assert.AreEqual(numerator.Uid, numerator1.NumeratorUid);
				Assert.AreEqual(true, numerator1.IsAutoNumbering);

				Assert.AreEqual(entity2, numerator2.EntityUid);
				Assert.AreEqual(numerator.Uid, numerator2.NumeratorUid);
				Assert.AreEqual(false, numerator2.IsAutoNumbering);

				Assert.AreEqual(entity3, numerator3.EntityUid);
				Assert.AreEqual(null, numerator3.NumeratorUid);
				Assert.AreEqual(true, numerator3.IsAutoNumbering);

				Assert.AreEqual(entity4, numerator4.EntityUid);
				Assert.AreEqual(null, numerator4.NumeratorUid);
				Assert.AreEqual(false, numerator4.IsAutoNumbering);

				var entityList = await getNumeratorEntityListHandler.Handle(new GetNumeratorEntityList { NumeratorUid = numerator.Uid }, cancellationToken);
				Assert.AreEqual(2, entityList.Rows.Count);
				Assert.AreEqual(1, entityList.Rows.Count(x => x.IsAutoNumbering));
				Assert.AreEqual(1, entityList.Rows.Count(x => x.IsAutoNumbering == false));
			}
		}
	}
}

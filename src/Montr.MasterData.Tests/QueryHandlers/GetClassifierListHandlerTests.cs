using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.QueryHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Moq;

namespace Montr.MasterData.Tests.QueryHandlers
{
	[TestClass]
	public class GetClassifierListHandlerTests
	{
		[TestMethod]
		public async Task GetClassifierList_Should_ReturnList()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var ctpMock = new Mock<INamedServiceFactory<IClassifierTypeProvider>>();
			ctpMock.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>()))
				.Returns(new ClassifierTypeProvider<Classifier>(dbContextFactory, null, null));

			var classifierRepository = new DbClassifierRepository(classifierTypeService, ctpMock.Object);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new GetClassifierListHandler(classifierRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);
				for (var i = 0; i < 42; i++)
				{
					await generator.InsertItem($"{i:D4}", null, cancellationToken);
				}

				// act
				var command = new GetClassifierList
				{
					UserUid = Guid.NewGuid(),
					TypeCode = generator.TypeCode
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.AreEqual(10, result.Rows.Count);
				Assert.AreEqual(42, result.TotalCount);
			}
		}
	}
}

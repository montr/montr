using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Services;
using Montr.Tendr.Impl.QueryHandlers;
using Montr.Tendr.Queries;
using Moq;

namespace Montr.Tendr.Tests.QueryHandlers
{
	[TestClass]
	public class GetInvitationListHandlerTests
	{
		[TestMethod]
		public async Task GetInvitationList_ForNormalRequest_ReturnItems()
		{
			// arrange
			var cancellationToken = CancellationToken.None;
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var ctpMock = new Mock<INamedServiceFactory<IClassifierTypeProvider>>();
			ctpMock.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>()))
				.Returns(new ClassifierTypeProvider(dbContextFactory, null, null));

			var classifierRepository = new DbClassifierRepository(classifierTypeService, ctpMock.Object);
			var handler = new GetInvitationListHandler(dbContextFactory, classifierRepository);

			// act
			var command = new GetInvitationList
			{
				UserUid = Guid.NewGuid(),
			};

			var result = await handler.Handle(command, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Rows);
		}
	}
}

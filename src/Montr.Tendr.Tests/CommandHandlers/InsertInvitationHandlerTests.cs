using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Services;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.CommandHandlers;
using Montr.Tendr.Models;
using Moq;

namespace Montr.Tendr.Tests.CommandHandlers
{
	[TestClass]
	public class InsertInvitationHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_InsertInvitation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, null);

			var ctpMock = new Mock<INamedServiceFactory<IClassifierTypeProvider>>();
			ctpMock.Setup(x => x.GetNamedOrDefaultService(It.IsAny<string>()))
				.Returns(new ClassifierTypeProvider<Classifier>(dbContextFactory, null, dbFieldDataRepository));

			var classifierRepository = new DbClassifierRepository(classifierTypeService, ctpMock.Object);
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertInvitationHandler(unitOfWorkFactory, dbContextFactory, classifierRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var command = new InsertInvitation
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					EventUid = Guid.Parse("436c290c-37b2-11e9-88fe-00ff279ba9e1"),
					Items = new []
					{
						new Invitation { CounterpartyUid = Guid.Parse("1bef28d6-2255-416c-a706-008e0c179508") }
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);
			}
		}
	}
}

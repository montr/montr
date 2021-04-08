using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Commands;
using Montr.Idx.Impl.CommandHandlers;
using Montr.Idx.Impl.Services;
using Montr.Idx.Models;
using Montr.Idx.Tests.Services;
using Moq;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestClass, Ignore]
	public class ExternalRegisterHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_RegisterExternalUser()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();

			var identityServiceFactory = new IdentityServiceFactory();

			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory) { UserTypeCode = User.TypeCode };
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();

			// todo: test EmailConfirmationService
			var emailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

			var handler = new ExternalRegisterHandler(new NullLogger<ExternalRegisterHandler>(), classifierRepositoryFactory,
				identityServiceFactory.UserManager, identityServiceFactory.SignInManager, emailConfirmationServiceMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var command = new ExternalRegister
				{
					Email = "test@montr.net"
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Message));
			}
		}
	}
}

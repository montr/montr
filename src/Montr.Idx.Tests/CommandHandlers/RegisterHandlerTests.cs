using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core;
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
	[TestClass]
	public class RegisterHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_RegisterUser()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();

			var appOptionsAccessorMock = new Mock<IOptionsMonitor<AppOptions>>();
			appOptionsAccessorMock.Setup(x => x.CurrentValue)
				.Returns(() => new AppOptions { AppUrl = "https://app.montr.net" });
			var appOptionsAccessor = appOptionsAccessorMock.Object;

			var identityServiceFactory = new IdentityServiceFactory();

			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory) { UserTypeCode = User.TypeCode };
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();

			// todo: test EmailConfirmationService
			/*
			var dbMessageTemplateRepository = new DbMessageTemplateRepository(dbContextFactory);
			var templateRenderer = new MustacheTemplateRenderer(dbMessageTemplateRepository);
			var emailConfirmationService = new EmailConfirmationService(userManager, appUrlBuilder, new Mock<IEmailSender>().Object, templateRenderer);
			*/

			var appUrlBuilder = new DefaultAppUrlBuilder(appOptionsAccessor);
			var emailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

			var generator = new IdxDbGenerator(unitOfWorkFactory, dbContextFactory);

			var handler = new RegisterHandler(new NullLogger<RegisterHandler>(), classifierRepositoryFactory,
				identityServiceFactory.UserManager, identityServiceFactory.SignInManager, appUrlBuilder, emailConfirmationServiceMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.EnsureUserTypeRegistered(cancellationToken);

				// act
				var command = new Register
				{
					Email = "test@montr.net",
					Password = Guid.NewGuid().ToString()
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsTrue(result.Success);
			}
		}
	}
}

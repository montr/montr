using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services.Implementations;
using Montr.Idx.Commands;
using Montr.Idx.Models;
using Montr.Idx.Services.CommandHandlers;
using Montr.Idx.Services.Implementations;
using Montr.Idx.Tests.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Tests.Services;
using Moq;
using NUnit.Framework;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestFixture]
	public class RegisterHandlerTests
	{
		[Test]
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

			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory) { UserTypeCode = ClassifierTypeCode.User };
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();

			// todo: test EmailConfirmationService
			/*
			var dbMessageTemplateRepository = new DbMessageTemplateRepository(dbContextFactory);
			var templateRenderer = new MustacheTemplateRenderer(dbMessageTemplateRepository);
			var emailConfirmationService = new EmailConfirmationService(userManager, appUrlBuilder, new Mock<IEmailSender>().Object, templateRenderer);
			*/

			var appUrlBuilder = new DefaultAppUrlBuilder(appOptionsAccessor);
			var emailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var handler = new RegisterHandler(NullLogger<RegisterHandler>.Instance, classifierRepositoryFactory,
				identityServiceFactory.UserManager, identityServiceFactory.SignInManager, appUrlBuilder, emailConfirmationServiceMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.EnsureClassifierTypeRegistered(Numerator.GetDefaultMetadata(), cancellationToken);
				await generator.EnsureClassifierTypeRegistered(User.GetDefaultMetadata(), cancellationToken);

				// act
				var command = new Register
				{
					Email = "test@montr.net",
					Password = Guid.NewGuid().ToString()
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.That(result.Success);
			}
		}
	}
}

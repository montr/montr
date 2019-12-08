using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Commands;
using Montr.Idx.Impl.CommandHandlers;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;
using Montr.Messages.Impl.Services;
using Montr.Messages.Services;
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
			var dbConnectionFactory = new DbConnectionFactory();
			var identityErrorDescriber = new IdentityErrorDescriber(); // todo: use own localized error describer

			var identityOptionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
			identityOptionsAccessorMock.Setup(x => x.Value).Returns(() => new IdentityOptions
			{
				Tokens = new TokenOptions
				{
					ProviderMap = new Dictionary<string, TokenProviderDescriptor>
					{
						{ TokenOptions.DefaultProvider, new TokenProviderDescriptor(typeof(DataProtectorTokenProvider<DbUser>)) },
						// { TokenOptions.DefaultEmailProvider, new TokenProviderDescriptor(typeof(EmailTokenProvider<DbUser>)) }
					}
				},

				User = new UserOptions
				{
					RequireUniqueEmail = true
				}
			});
			var identityOptionsAccessor = identityOptionsAccessorMock.Object;

			var appOptionsAccessorMock = new Mock<IOptionsMonitor<AppOptions>>();
			appOptionsAccessorMock.Setup(x => x.CurrentValue).Returns(() => new AppOptions
			{
				AppUrl = "https://app.montr.net"
			});
			var appOptionsAccessor = appOptionsAccessorMock.Object;

			var lookupNormalizer = new UpperInvariantLookupNormalizer();

			var dataProtectorTokenProvider = new DataProtectorTokenProvider<DbUser>(
				new EphemeralDataProtectionProvider(new NullLoggerFactory()), null, new NullLogger<DataProtectorTokenProvider<DbUser>>());

			var serviceProviderMock = new Mock<IServiceProvider>();

			var serviceScopeMock = new Mock<IServiceScope>();
			serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

			var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
			serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

			var authenticationServiceMock = new Mock<IAuthenticationService>();

			serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
			serviceProviderMock.Setup(x => x.GetService(typeof(DataProtectorTokenProvider<DbUser>))).Returns(dataProtectorTokenProvider);
			serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(authenticationServiceMock.Object);

			var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
			var httpContext = new DefaultHttpContext { ServiceScopeFactory = serviceScopeFactoryMock.Object };
			httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
			var httpContextAccessor = httpContextAccessorMock.Object;

			var userStore = new UserStore<Guid, DbUser, DbRole>(dbConnectionFactory, identityErrorDescriber);
			var roleStore = new RoleStore<Guid, DbRole>(dbConnectionFactory, identityErrorDescriber);

			var userManager = new UserManager<DbUser>(userStore, identityOptionsAccessor, new PasswordHasher<DbUser>(), null, null, lookupNormalizer, identityErrorDescriber, serviceProviderMock.Object, new NullLogger<UserManager<DbUser>>());
			userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, dataProtectorTokenProvider);

			var roleManager = new AspNetRoleManager<DbRole>(roleStore, null, lookupNormalizer, identityErrorDescriber, new NullLogger<RoleManager<DbRole>>(), httpContextAccessor);
			var userClaimsPrincipalFactory = new UserClaimsPrincipalFactory<DbUser,DbRole>(userManager, roleManager, identityOptionsAccessor);
			var signInManager = new SignInManager<DbUser>(userManager, httpContextAccessor, userClaimsPrincipalFactory, identityOptionsAccessor, new NullLogger<SignInManager<DbUser>>(), null, null);

			var appUrlBuilder = new DefaultAppUrlBuilder(appOptionsAccessor);
			var templateRenderer = new MustacheTemplateRenderer(dbContextFactory);
			var emailConfirmationService = new EmailConfirmationService(userManager, appUrlBuilder, new Mock<IEmailSender>().Object, templateRenderer);

			var handler = new RegisterHandler(
				new Mock<ILogger<RegisterHandler>>().Object,
				userManager, signInManager, appUrlBuilder, emailConfirmationService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange

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

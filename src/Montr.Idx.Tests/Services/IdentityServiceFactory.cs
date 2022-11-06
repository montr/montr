using System;
using System.Collections.Generic;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Montr.Idx.Entities;
using Montr.Idx.Services.Implementations;
using Moq;

namespace Montr.Idx.Tests.Services
{
	public class IdentityServiceFactory
	{
		public UserManager<DbUser> UserManager { get; }

		public RoleManager<DbRole> RoleManager { get; }

		public SignInManager<DbUser> SignInManager { get; }

		public IdentityServiceFactory()
		{
			var dataProtectorTokenProvider = new DataProtectorTokenProvider<DbUser>(
				new EphemeralDataProtectionProvider(new NullLoggerFactory()), null, new NullLogger<DataProtectorTokenProvider<DbUser>>());

			// service provider
			var serviceProviderMock = new Mock<IServiceProvider>();

			var serviceScopeMock = new Mock<IServiceScope>();
			serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

			var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
			serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

			var authenticationServiceMock = new Mock<IAuthenticationService>();

			serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
			serviceProviderMock.Setup(x => x.GetService(typeof(DataProtectorTokenProvider<DbUser>))).Returns(dataProtectorTokenProvider);
			serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(authenticationServiceMock.Object);

			// http context
			var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
			var httpContext = new DefaultHttpContext { ServiceScopeFactory = serviceScopeFactoryMock.Object };
			httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
			var httpContextAccessor = httpContextAccessorMock.Object;

			// identity services
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

			var identityErrorDescriber = new IdentityErrorDescriber();
			var lookupNormalizer = new UpperInvariantLookupNormalizer();

			var connectionFactory = new DbConnectionFactory();

			var userStore = new UserStore<Guid, DbUser, DbRole>(connectionFactory, identityErrorDescriber);
			var roleStore = new RoleStore<Guid, DbRole>(connectionFactory, identityErrorDescriber);

			var userManager = new UserManager<DbUser>(userStore, identityOptionsAccessor, new PasswordHasher<DbUser>(), null, null,
				lookupNormalizer, identityErrorDescriber, serviceProviderMock.Object, new NullLogger<UserManager<DbUser>>());
			userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, dataProtectorTokenProvider);

			var roleManager = new AspNetRoleManager<DbRole>(roleStore, null, lookupNormalizer, identityErrorDescriber,
				new NullLogger<RoleManager<DbRole>>(), httpContextAccessor);
			var userClaimsPrincipalFactory = new UserClaimsPrincipalFactory<DbUser,DbRole>(userManager, roleManager, identityOptionsAccessor);
			var signInManager = new SignInManager<DbUser>(userManager, httpContextAccessor, userClaimsPrincipalFactory, identityOptionsAccessor,
				new NullLogger<SignInManager<DbUser>>(), null, null);

			UserManager = userManager;
			RoleManager = roleManager;
			SignInManager = signInManager;
		}
	}
}

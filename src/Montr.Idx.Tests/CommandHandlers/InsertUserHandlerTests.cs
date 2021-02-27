using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.CommandHandlers;
using Montr.Idx.Impl.Services;
using Montr.Idx.Models;
using Montr.Idx.Tests.Services;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestClass]
	public class InsertUserHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_InsertUser()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbConnectionFactory = new DbConnectionFactory();

			var identityServiceFactory = new IdentityServiceFactory(dbConnectionFactory);
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var handler = new InsertUserHandler(unitOfWorkFactory, userManager);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var request = new InsertUser
				{
					Item = new User
					{
						Email = "test@montr.net"
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

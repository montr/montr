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
	public class UpdateUserHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_UpdateUser()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbConnectionFactory = new DbConnectionFactory();

			var identityServiceFactory = new IdentityServiceFactory(dbConnectionFactory);
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var handler = new UpdateUserHandler(userManager);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var createResult = await userManager.Create(new User
				{
					UserName = "test@montr.net",
					Email = "test@montr.net"
				}, cancellationToken);
				// ReSharper disable once PossibleInvalidOperationException
				var user = await userManager.Get(createResult.Uid.Value, cancellationToken);

				// act
				var request = new UpdateUser
				{
					Item = new User
					{
						Uid = user.Uid,
						UserName = user.UserName,
						Email = user.Email,
						FirstName = "John",
						LastName = "Smith",
						SecurityStamp = user.SecurityStamp
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

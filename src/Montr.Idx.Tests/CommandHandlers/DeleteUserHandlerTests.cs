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
	public class DeleteUserHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_DeleteUser()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();

			var identityServiceFactory = new IdentityServiceFactory();
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var handler = new DeleteUserHandler(unitOfWorkFactory, userManager);

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
				var result = await handler.Handle(new DeleteUser { Item = user }, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

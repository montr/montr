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
	public class DeleteRoleHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_DeleteRole()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();

			var identityServiceFactory = new IdentityServiceFactory();
			var roleManager = new DefaultRoleManager(new NullLogger<DefaultRoleManager>(), identityServiceFactory.RoleManager);

			var handler = new DeleteRoleHandler(unitOfWorkFactory, roleManager);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var createResult = await roleManager.Create(new Role
				{
					Name = "test_role"
				}, cancellationToken);

				// ReSharper disable once PossibleInvalidOperationException
				var role = await roleManager.Get(createResult.Uid.Value, cancellationToken);

				// act
				var result = await handler.Handle(new DeleteRole { Item = role }, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

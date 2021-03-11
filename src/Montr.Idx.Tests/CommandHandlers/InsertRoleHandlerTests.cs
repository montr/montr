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
	public class InsertRoleHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_InsertRole()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbConnectionFactory = new DbConnectionFactory();

			var identityServiceFactory = new IdentityServiceFactory(dbConnectionFactory);
			var roleManager = new DefaultRoleManager(new NullLogger<DefaultRoleManager>(), identityServiceFactory.RoleManager);

			var handler = new InsertRoleHandler(unitOfWorkFactory, roleManager);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act
				var request = new InsertRole
				{
					Item = new Role
					{
						Name = "test_role"
					}
				};

				var result = await handler.Handle(request, cancellationToken);

				// assert
				Assert.IsTrue(result.Success, string.Join(",", result.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

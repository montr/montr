using System;
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
	public class ManageUserRolesHandlerTests
	{
		[TestMethod]
		public async Task ManageUserRoles_NormalValues_ReturnSuccess()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();

			var identityServiceFactory = new IdentityServiceFactory();
			var roleManager = new DefaultRoleManager(new NullLogger<DefaultRoleManager>(), identityServiceFactory.RoleManager);
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var addHandler = new AddUserRolesHandler(unitOfWorkFactory, userManager);
			var removeHandler = new RemoveUserRolesHandler(unitOfWorkFactory, userManager);

			var roles = new[] { "test_role_1", "test_role_2", "test_role_3" };

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				foreach (var name in roles)
				{
					var role = await roleManager.Create(new Role { Uid = Guid.NewGuid(), Name = name }, cancellationToken);
					Assert.IsTrue(role.Success);
				}

				// var dbRoles = identityServiceFactory.RoleManager.Roles.ToList();

				var user = await userManager.Create(new User { Uid = Guid.NewGuid(), UserName = "test_user" }, cancellationToken);
				Assert.IsTrue(user.Success);

				// ReSharper disable once PossibleInvalidOperationException
				var userUid = user.Uid.Value;

				// act - add roles
				var addResult = await addHandler.Handle(new AddUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.IsTrue(addResult.Success);

				// act - remove roles
				var removeResult = await removeHandler.Handle(new RemoveUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.IsTrue(removeResult.Success);
			}
		}
	}
}

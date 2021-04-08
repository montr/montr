using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Commands;
using Montr.Idx.Impl.CommandHandlers;
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
			var dbContextFactory = new DefaultDbContextFactory();

			var identityServiceFactory = new IdentityServiceFactory();
			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory);
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();

			var roleRepository = classifierRepositoryFactory.GetNamedOrDefaultService(classifierRepositoryFactoryBuilder.RoleTypeCode);
			var userRepository = classifierRepositoryFactory.GetNamedOrDefaultService(classifierRepositoryFactoryBuilder.UserTypeCode);

			var addHandler = new AddUserRolesHandler(new NullLogger<AddUserRolesHandler>(), unitOfWorkFactory, identityServiceFactory.UserManager);
			var removeHandler = new RemoveUserRolesHandler(new NullLogger<RemoveUserRolesHandler>(), unitOfWorkFactory, identityServiceFactory.UserManager);

			var roles = new[] { "test_role_1", "test_role_2", "test_role_3" };

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				foreach (var name in roles)
				{
					var role = await roleRepository.Insert(new Role { Uid = Guid.NewGuid(), Name = name }, cancellationToken);
					Assert.IsTrue(role.Success, string.Join(",", role.Errors.SelectMany(x => x.Messages)));
				}

				// var dbRoles = identityServiceFactory.RoleManager.Roles.ToList();

				var user = await userRepository.Insert(new User { Uid = Guid.NewGuid(), UserName = "test_user" }, cancellationToken);
				Assert.IsTrue(user.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));

				// ReSharper disable once PossibleInvalidOperationException
				var userUid = user.Uid.Value;

				// act - add roles
				var addResult = await addHandler.Handle(new AddUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.IsTrue(addResult.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));

				// act - remove roles
				var removeResult = await removeHandler.Handle(new RemoveUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.IsTrue(removeResult.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services.Implementations;
using Montr.Idx.Commands;
using Montr.Idx.Models;
using Montr.Idx.Services.CommandHandlers;
using Montr.Idx.Tests.Services;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestFixture]
	public class ManageUserRolesHandlerTests
	{
		[Test]
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

			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);

			var addHandler = new AddUserRolesHandler(NullLogger<AddUserRolesHandler>.Instance, unitOfWorkFactory, identityServiceFactory.UserManager);
			var removeHandler = new RemoveUserRolesHandler(NullLogger<RemoveUserRolesHandler>.Instance, unitOfWorkFactory, identityServiceFactory.UserManager);

			var roles = new[] { "test_role_1", "test_role_2", "test_role_3" };

			using (var _ = unitOfWorkFactory.Create())
			{
				await generator.EnsureClassifierTypeRegistered(Role.GetDefaultMetadata(), cancellationToken);
				await generator.EnsureClassifierTypeRegistered(User.GetDefaultMetadata(), cancellationToken);

				// arrange
				foreach (var name in roles)
				{
					var role = await roleRepository.Insert(new Role { Uid = Guid.NewGuid(), Name = name }, cancellationToken);
					Assert.That(role.Success, string.Join(",", role.Errors.SelectMany(x => x.Messages)));
				}

				// var dbRoles = identityServiceFactory.RoleManager.Roles.ToList();

				var user = await userRepository.Insert(new User { Uid = Guid.NewGuid(), UserName = "test_user" }, cancellationToken);
				Assert.That(user.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));

				// ReSharper disable once PossibleInvalidOperationException
				var userUid = user.Uid.Value;

				// act - add roles
				var addResult = await addHandler.Handle(new AddUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.That(addResult.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));

				// act - remove roles
				var removeResult = await removeHandler.Handle(new RemoveUserRoles { UserUid = userUid, Roles = roles }, cancellationToken);
				Assert.That(removeResult.Success, string.Join(",", user.Errors.SelectMany(x => x.Messages)));
			}
		}
	}
}

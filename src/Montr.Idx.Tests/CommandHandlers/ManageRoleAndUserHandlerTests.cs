using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Services;
using Montr.Idx.Models;
using Montr.Idx.Tests.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestClass]
	public class ManageRoleAndUserHandlerTests
	{
		private static readonly string RoleTypeCode = "role_for_test";
		private static readonly string UserTypeCode = "user_for_test";

		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
		{
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new FieldMetadata[]
				{
					/*new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }*/
				});

			var identityServiceFactory = new IdentityServiceFactory();

			var roleManager = new DefaultRoleManager(new NullLogger<DefaultRoleManager>(), identityServiceFactory.RoleManager);
			var userManager = new DefaultUserManager(new NullLogger<DefaultUserManager>(), identityServiceFactory.UserManager);

			var roleRepository = new DbRoleRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, roleManager);

			var userRepository = new DbUserRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null, userManager);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == RoleTypeCode)))
				.Returns(() => roleRepository);

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == UserTypeCode)))
				.Returns(() => userRepository);

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name != RoleTypeCode && name != UserTypeCode )))
				.Throws<InvalidOperationException>();

			return classifierRepositoryFactoryMock.Object;
		}

		[TestMethod]
		public async Task ManageRole_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var insertHandler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var updateHandler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var deleteHandler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = RoleTypeCode;

				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act - insert
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 3; i++)
				{
					var insertResult = await insertHandler.Handle(new InsertClassifier
					{
						UserUid = generator.UserUid,
						Item = new Role
						{
							Type = generator.TypeCode,
							Code = "00" + i,
							Name = "00" + i + " - Test Role"
						}
					}, cancellationToken);

					Assert.IsNotNull(insertResult);
					Assert.IsTrue(insertResult.Success, string.Join(",", insertResult.Errors.SelectMany(x => x.Messages)));

					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertResult.Uid.Value);
				}

				var searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);

				// act - update
				foreach (var item in searchResult.Rows.Cast<Role>())
				{
					item.Name = item.Name.Replace("Test", "Updated");

					var updateCommand = new UpdateClassifier
					{
						UserUid = generator.UserUid,
						Item = item
					};

					var updateResult = await updateHandler.Handle(updateCommand, cancellationToken);

					Assert.IsNotNull(updateResult);
					Assert.IsTrue(updateResult.Success, string.Join(",", updateResult.Errors.SelectMany(x => x.Messages)));
				}

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);
				Assert.AreEqual(0, searchResult.Rows.Count(x => x.Name.Contains("Test")));
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count(x => x.Name.Contains("Updated")));

				// act - delete
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};

				var deleteResult = await deleteHandler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(deleteResult);
				Assert.IsTrue(deleteResult.Success, string.Join(",", deleteResult.Errors?.SelectMany(x => x.Messages) ?? Array.Empty<string>()));
				Assert.AreEqual(insertedIds.Count, deleteResult.AffectedRows);

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(0, searchResult.Rows.Count);
			}
		}

		[TestMethod]
		public async Task ManageUser_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var insertHandler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var updateHandler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var deleteHandler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = UserTypeCode;

				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act - insert
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 3; i++)
				{
					var insertResult = await insertHandler.Handle(new InsertClassifier
					{
						UserUid = generator.UserUid,
						Item = new User
						{
							Type = generator.TypeCode,
							Code = "00" + i,
							Name = "00" + i + " - Test User",
							UserName = "00" + i + " - Test UserName"
						}
					}, cancellationToken);

					Assert.IsNotNull(insertResult);
					Assert.IsTrue(insertResult.Success, string.Join(",", insertResult.Errors.SelectMany(x => x.Messages)));

					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertResult.Uid.Value);
				}

				var searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);

				// act - update
				foreach (var item in searchResult.Rows.Cast<User>())
				{
					item.Name = item.Name.Replace("Test", "Updated");

					var updateCommand = new UpdateClassifier
					{
						UserUid = generator.UserUid,
						Item = item
					};

					var updateResult = await updateHandler.Handle(updateCommand, cancellationToken);

					Assert.IsNotNull(updateResult);
					Assert.IsTrue(updateResult.Success, string.Join(",", updateResult.Errors.SelectMany(x => x.Messages)));
				}

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);
				Assert.AreEqual(0, searchResult.Rows.Count(x => x.Name.Contains("Test")));
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count(x => x.Name.Contains("Updated")));

				// act - delete
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};

				var deleteResult = await deleteHandler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(deleteResult);
				Assert.IsTrue(deleteResult.Success, string.Join(",", deleteResult.Errors?.SelectMany(x => x.Messages) ?? Array.Empty<string>()));
				Assert.AreEqual(insertedIds.Count, deleteResult.AffectedRows);

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(0, searchResult.Rows.Count);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Idx.Models;
using Montr.Idx.Tests.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Models;
using Montr.MasterData.Tests.Services;
using NUnit.Framework;

namespace Montr.Idx.Tests.CommandHandlers
{
	[TestFixture]
	public class ManageRoleAndUserHandlerTests
	{
		[Test]
		public async Task ManageRole_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory);
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();
			var insertHandler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var updateHandler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var deleteHandler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = classifierRepositoryFactoryBuilder.RoleTypeCode;

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

		[Test]
		public async Task ManageUser_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactoryBuilder = new ClassifierRepositoryFactoryBuilder(dbContextFactory);
			var classifierRepositoryFactory = classifierRepositoryFactoryBuilder.Build();
			var insertHandler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var updateHandler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);
			var deleteHandler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = classifierRepositoryFactoryBuilder.UserTypeCode;

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

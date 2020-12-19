using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Moq;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class ClassifierHandlerTests
	{
		private static readonly string TypedClassifierTypeCode = "typed_test";

		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(
			IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			var dateTimeProvider = new DefaultDateTimeProvider();
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
					new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }
				});

			var classifierRepository = new DbClassifierRepository<Classifier>(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, classifierTypeService, null, metadataServiceMock.Object,
				dbFieldDataRepository, null);

			var numeratorRepository = new DbNumeratorRepository(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, classifierTypeService, null, metadataServiceMock.Object,
				dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == TypedClassifierTypeCode)))
				.Returns(() => numeratorRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name != TypedClassifierTypeCode)))
				.Returns(() => classifierRepository);

			return classifierRepositoryFactoryMock.Object;
		}

		[TestMethod]
		public async Task InsertClassifier_NormalValues_InsertItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var factory = CreateClassifierRepositoryFactory(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierHandler(factory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act
				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					Item = new Classifier
					{
						Type = generator.TypeCode,
						Code = "001",
						Name = "Test Classifier",
						Fields = new FieldData
						{
							{ "test1", "value1" },
							{ "test2", "value2" },
							{ "test3", "value3" }
						}
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);

				var classifierResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode, Uid = result.Uid, IncludeFields = true }, cancellationToken);

				Assert.AreEqual(1, classifierResult.Rows.Count);
				var inserted = classifierResult.Rows[0];
				// todo: figure out why insert.Fields is null
				Assert.AreEqual(command.Item.Code, inserted.Code);
				Assert.AreEqual(command.Item.Name, inserted.Name);

				// assert field data inserted
				IList<DbFieldData> fieldData;
				using (var db = dbContextFactory.Create())
				{
					fieldData = await db.GetTable<DbFieldData>()
						.Where(x => x.EntityTypeCode == Classifier.TypeCode && x.EntityUid == result.Uid)
						.ToListAsync(cancellationToken);
				}

				Assert.AreEqual(command.Item.Fields.Count, fieldData.Count);
				Assert.AreEqual(command.Item.Fields["test1"], fieldData.Single(x => x.Key == "test1").Value);
				Assert.AreEqual(command.Item.Fields["test2"], fieldData.Single(x => x.Key == "test2").Value);
				Assert.AreEqual(command.Item.Fields["test3"], fieldData.Single(x => x.Key == "test3").Value);
			}
		}

		[TestMethod]
		public async Task InsertClassifier_DuplicateCode_ReturnError()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var factory = CreateClassifierRepositoryFactory(unitOfWorkFactory, dbContextFactory);
			var handler = new InsertClassifierHandler(factory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					Item = new Classifier
					{
						Type = generator.TypeCode,
						Code = "001",
						Name = "Test Classifier"
					}
				};

				// act
				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);

				// act
				result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.IsNull(result.Uid);
				Assert.AreEqual(1, result.Errors.Count);
				Assert.AreEqual("code", result.Errors[0].Key);
				// todo: use error codes?
				Assert.AreEqual("Код «001» уже используется в элементе «Test Classifier».",
					result.Errors[0].Messages[0]);
			}
		}

		[TestMethod]
		public async Task UpdateClassifier_NormalValues_UpdateItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var factory = CreateClassifierRepositoryFactory(unitOfWorkFactory, dbContextFactory);
			var handler = new UpdateClassifierHandler(factory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);
				var insertItem = await generator.InsertItem("001", null, cancellationToken);

				// act
				var command = new UpdateClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					Item = new Classifier
					{
						// ReSharper disable once PossibleInvalidOperationException
						Uid = insertItem.Uid.Value,
						Type = generator.TypeCode,
						Code = "001",
						Name = "Test Classifier",
						Fields = new FieldData
						{
							{ "test1", "value1" },
							{ "test2", "value2" },
							{ "test3", "value3" }
						}
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);
			}
		}

		[TestMethod]
		public async Task DeleteClassifier_NormalValues_DeleteItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var factory = CreateClassifierRepositoryFactory(unitOfWorkFactory, dbContextFactory);
			var handler = new DeleteClassifierHandler(factory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 5; i++)
				{
					var insertItem = await generator.InsertItem("00" + i, null, cancellationToken);
					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertItem.Uid.Value);
				}

				var searchResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);

				// act
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};
				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(insertedIds.Count, result.AffectedRows);

				searchResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(0, searchResult.Rows.Count);
			}
		}

		[TestMethod]
		public async Task ManageNumerator_NormalValues_ManageItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var factory = CreateClassifierRepositoryFactory(unitOfWorkFactory, dbContextFactory);
			var insertHandler = new InsertClassifierHandler(factory);
			var updateHandler = new UpdateClassifierHandler(factory);
			var deleteHandler = new DeleteClassifierHandler(factory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				generator.TypeCode = TypedClassifierTypeCode;

				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act - insert
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 5; i++)
				{
					var insertResult = await insertHandler.Handle(new InsertClassifier
					{
						UserUid = generator.UserUid,
						CompanyUid = generator.CompanyUid,
						Item = new Numerator
						{
							Type = generator.TypeCode,
							Code = "00" + i,
							Name = "00" + i + " - Test Numerator",
							EntityTypeCode = "DocumentType",
							Pattern = "{Number}"
						}
					}, cancellationToken);

					Assert.IsNotNull(insertResult);
					Assert.AreEqual(true, insertResult.Success);

					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertResult.Uid.Value);
				}

				var searchResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);

				// act - update
				foreach (var classifier in searchResult.Rows.Cast<Numerator>())
				{
					classifier.Name = classifier.Name.Replace("Test", "Updated");
					classifier.Pattern = "No. " + classifier.Pattern;

					var updateCommand = new UpdateClassifier
					{
						UserUid = generator.UserUid,
						CompanyUid = generator.CompanyUid,
						Item = classifier
					};

					var updateResult = await updateHandler.Handle(updateCommand, cancellationToken);

					Assert.IsNotNull(updateResult);
					Assert.AreEqual(true, updateResult.Success);
				}

				searchResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count);
				Assert.AreEqual(0, searchResult.Rows.Count(x => x.Name.Contains("Test")));
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Count(x => x.Name.Contains("Updated")));
				Assert.AreEqual(insertedIds.Count, searchResult.Rows.Cast<Numerator>().Count(x => x.Pattern.Contains("No.")));

				// act - delete
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};

				var result = await deleteHandler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(insertedIds.Count, result.AffectedRows);

				searchResult = await factory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.IsNotNull(searchResult);
				Assert.AreEqual(0, searchResult.Rows.Count);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging.Abstractions;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.CommandHandlers;
using Montr.MasterData.Services.Implementations;
using Montr.MasterData.Tests.Services;
using Montr.Metadata.Entities;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestFixture]
	public class ClassifierHandlerTests
	{
		private static readonly string NumeratorTypeCode = "numerator_for_test";

		private static INamedServiceFactory<IClassifierRepository> CreateClassifierRepositoryFactory(IDbContextFactory dbContextFactory)
		{
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);

			var fieldProviderRegistry = new DefaultFieldProviderRegistry();
			fieldProviderRegistry.AddFieldType(typeof(TextField));
			var dbFieldDataRepository = new DbFieldDataRepository(NullLogger<DbFieldDataRepository>.Instance, dbContextFactory, fieldProviderRegistry);

			var metadataServiceMock = new Mock<IClassifierTypeMetadataService>();
			metadataServiceMock
				.Setup(x => x.GetMetadata(It.IsAny<ClassifierType>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new FieldMetadata[]
				{
					new TextField { Key = "test1", Active = true, System = false },
					new TextField { Key = "test2", Active = true, System = false },
					new TextField { Key = "test3", Active = true, System = false }
				});

			var classifierRepository = new DbClassifierRepository<Classifier>(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var numeratorRepository = new DbNumeratorRepository(dbContextFactory,
				classifierTypeService, null, metadataServiceMock.Object, dbFieldDataRepository, null);

			var classifierRepositoryFactoryMock = new Mock<INamedServiceFactory<IClassifierRepository>>();

			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name == NumeratorTypeCode)))
				.Returns(() => numeratorRepository);
			classifierRepositoryFactoryMock
				.Setup(x => x.GetNamedOrDefaultService(It.Is<string>(name => name != NumeratorTypeCode)))
				.Returns(() => classifierRepository);

			return classifierRepositoryFactoryMock.Object;
		}

		[Test]
		public async Task InsertClassifier_NormalValues_InsertItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act
				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.Uid, Is.Not.Null);
				Assert.That(result.Uid, Is.Not.EqualTo(Guid.Empty));

				var classifierResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode, Uid = result.Uid, IncludeFields = true }, cancellationToken);

				Assert.That(classifierResult.Rows, Has.Count.EqualTo(1));
				var inserted = classifierResult.Rows[0];
				// todo: figure out why insert.Fields is null
				Assert.That(inserted.Code, Is.EqualTo(command.Item.Code));
				Assert.That(inserted.Name, Is.EqualTo(command.Item.Name));

				// assert field data inserted
				IList<DbFieldData> fieldData;
				using (var db = dbContextFactory.Create())
				{
					fieldData = await db.GetTable<DbFieldData>()
						.Where(x => x.EntityTypeCode == EntityTypeCode.Classifier && x.EntityUid == result.Uid)
						.ToListAsync(cancellationToken);
				}

				Assert.That(fieldData, Has.Count.EqualTo(command.Item.Fields.Count));
				Assert.That(fieldData.Single(x => x.Key == "test1").Value, Is.EqualTo(command.Item.Fields["test1"]));
				Assert.That(fieldData.Single(x => x.Key == "test2").Value, Is.EqualTo(command.Item.Fields["test2"]));
				Assert.That(fieldData.Single(x => x.Key == "test3").Value, Is.EqualTo(command.Item.Fields["test3"]));
			}
		}

		[Test]
		public async Task InsertClassifier_DuplicateCode_ReturnError()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange - 1st item
				await generator.InsertType(HierarchyType.None, cancellationToken);

				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.Uid, Is.Not.Null);
				Assert.That(result.Uid, Is.Not.EqualTo(Guid.Empty));

				// arrange - 2nd item
				command.Item.Uid = Guid.NewGuid();

				// act
				result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success, Is.False);
				Assert.That(result.Uid, Is.Null);
				Assert.That(result.Errors, Has.Count.EqualTo(1));
				Assert.That(result.Errors[0].Key, Is.EqualTo("code"));
				// todo: use error codes?
				Assert.That(result.Errors[0].Messages[0], Is.EqualTo("Code «001» is already used in item \"Test Classifier\""));
			}
		}

		[Test]
		public async Task UpdateClassifier_NormalValues_UpdateItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var handler = new UpdateClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);
				var insertItem = await generator.InsertItem("001", null, cancellationToken);

				// act
				var command = new UpdateClassifier
				{
					UserUid = generator.UserUid,
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
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task DeleteClassifier_NormalValues_DeleteItems()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var generator = new MasterDataDbGenerator(unitOfWorkFactory, dbContextFactory);
			var classifierRepositoryFactory = CreateClassifierRepositoryFactory(dbContextFactory);
			var handler = new DeleteClassifierHandler(unitOfWorkFactory, classifierRepositoryFactory);

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

				var searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Has.Count.EqualTo(insertedIds.Count));

				// act
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};
				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(insertedIds.Count));

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Is.Empty);
			}
		}

		[Test]
		public async Task ManageNumerator_NormalValues_ManageItems()
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
				generator.TypeCode = NumeratorTypeCode;

				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act - insert
				var insertedIds = new List<Guid>();
				for (var i = 0; i < 5; i++)
				{
					var insertResult = await insertHandler.Handle(new InsertClassifier
					{
						UserUid = generator.UserUid,
						Item = new Numerator
						{
							Type = generator.TypeCode,
							Code = "00" + i,
							Name = "00" + i + " - Test Numerator",
							EntityTypeCode = "DocumentType",
							Pattern = "{Number}"
						}
					}, cancellationToken);

					Assert.That(insertResult, Is.Not.Null);
					Assert.That(insertResult.Success, Is.EqualTo(true));

					// ReSharper disable once PossibleInvalidOperationException
					insertedIds.Add(insertResult.Uid.Value);
				}

				var searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Has.Count.EqualTo(insertedIds.Count));

				// act - update
				foreach (var classifier in searchResult.Rows.Cast<Numerator>())
				{
					classifier.Name = classifier.Name.Replace("Test", "Updated");
					classifier.Pattern = "No. " + classifier.Pattern;

					var updateCommand = new UpdateClassifier
					{
						UserUid = generator.UserUid,
						Item = classifier
					};

					var updateResult = await updateHandler.Handle(updateCommand, cancellationToken);

					Assert.That(updateResult, Is.Not.Null);
					Assert.That(updateResult.Success, Is.EqualTo(true));
				}

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Has.Count.EqualTo(insertedIds.Count));
				Assert.That(searchResult.Rows.Count(x => x.Name.Contains("Test")), Is.EqualTo(0));
				Assert.That(searchResult.Rows.Count(x => x.Name.Contains("Updated")), Is.EqualTo(insertedIds.Count));
				Assert.That(searchResult.Rows.Cast<Numerator>().Count(x => x.Pattern.Contains("No.")), Is.EqualTo(insertedIds.Count));

				// act - delete
				var command = new DeleteClassifier
				{
					UserUid = generator.UserUid,
					TypeCode = generator.TypeCode,
					Uids = insertedIds.ToArray()
				};

				var result = await deleteHandler.Handle(command, cancellationToken);

				// assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Success);
				Assert.That(result.AffectedRows, Is.EqualTo(insertedIds.Count));

				searchResult = await classifierRepositoryFactory.GetNamedOrDefaultService(generator.TypeCode)
					.Search(new ClassifierSearchRequest { TypeCode = generator.TypeCode }, cancellationToken);

				// assert
				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.Rows, Is.Empty);
			}
		}
	}
}

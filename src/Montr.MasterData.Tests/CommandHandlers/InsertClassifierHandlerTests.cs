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
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Moq;

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class InsertClassifierHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_InsertClassifier()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
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
			var classifierRepository = new DbClassifierRepository(dbContextFactory, classifierTypeService, metadataServiceMock.Object, dbFieldDataRepository);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory,
				dateTimeProvider, classifierTypeService, metadataServiceMock.Object, dbFieldDataRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.None, cancellationToken);

				// act
				var command = new InsertClassifier
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					Item = new Classifier
					{
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

				var classifierResult = await classifierRepository
					.Search(new ClassifierSearchRequest { TypeCode = dbHelper.TypeCode, Uid = result.Uid, IncludeFields = true }, cancellationToken);

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
						.Where(x => x.EntityTypeCode == Classifier.EntityTypeCode && x.EntityUid == result.Uid)
						.ToListAsync(cancellationToken);
				}

				Assert.AreEqual(command.Item.Fields.Count, fieldData.Count);
				Assert.AreEqual(command.Item.Fields["test1"], fieldData.Single(x => x.Key == "test1").Value);
				Assert.AreEqual(command.Item.Fields["test2"], fieldData.Single(x => x.Key == "test2").Value);
				Assert.AreEqual(command.Item.Fields["test3"], fieldData.Single(x => x.Key == "test3").Value);
			}
		}

		[TestMethod]
		public async Task Handle_DuplicateCode_ReturnError()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DbClassifierTypeService(dbContextFactory, classifierTypeRepository);
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, null, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, null);
			var classifierTypeMetadataService = new ClassifierTypeMetadataService(dbFieldMetadataRepository);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService, classifierTypeMetadataService, dbFieldDataRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await dbHelper.InsertType(HierarchyType.None, cancellationToken);

				var command = new InsertClassifier
				{
					UserUid = dbHelper.UserUid,
					CompanyUid = dbHelper.CompanyUid,
					TypeCode = dbHelper.TypeCode,
					Item = new Classifier
					{
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
				Assert.AreEqual("Код «001» уже используется в элементе «Test Classifier».", result.Errors[0].Messages[0]);
			}
		}
	}
}

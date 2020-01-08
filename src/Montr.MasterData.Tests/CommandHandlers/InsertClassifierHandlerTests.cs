using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Impl.Services;

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
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var generator = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, null, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, null);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService, dbFieldMetadataRepository, dbFieldDataRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				// act
				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
					Item = new Classifier
					{
						Code = "001",
						Name = "Test Classifier"
					}
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);
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
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var generator = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var dbFieldMetadataRepository = new DbFieldMetadataRepository(dbContextFactory, null, new NewtonsoftJsonSerializer());
			var dbFieldDataRepository = new DbFieldDataRepository(dbContextFactory, null);
			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService, dbFieldMetadataRepository, dbFieldDataRepository);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				await generator.InsertType(HierarchyType.None, cancellationToken);

				var command = new InsertClassifier
				{
					UserUid = generator.UserUid,
					CompanyUid = generator.CompanyUid,
					TypeCode = generator.TypeCode,
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

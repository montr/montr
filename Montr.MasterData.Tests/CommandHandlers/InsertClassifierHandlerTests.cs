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

namespace Montr.MasterData.Tests.CommandHandlers
{
	[TestClass]
	public class InsertClassifierHandlerTests
	{
		[TestMethod]
		public async Task InsertClassifier_Should_InsertClassifier()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService);

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
		public async Task InsertClassifier_ShouldThrow_WhenDuplicateCodeInserted()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);
			var generator = new DbGenerator(unitOfWorkFactory, dbContextFactory);

			var cancellationToken = new CancellationToken();

			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider, classifierTypeService);

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

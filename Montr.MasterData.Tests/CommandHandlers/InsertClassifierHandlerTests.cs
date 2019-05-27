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
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory { Commitable = false };
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var classifierTypeRepository = new DbClassifierTypeRepository(dbContextFactory);
			var classifierTypeService = new DefaultClassifierTypeService(classifierTypeRepository);

			var handler = new InsertClassifierHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider, classifierTypeService);

			// act
			var command = new InsertClassifier
			{
				UserUid = Guid.NewGuid(),
				CompanyUid = Constants.OperatorCompanyUid,
				TypeCode = "test",
				Item = new Classifier
				{
					// TypeCode = "test",
					Code = "001",
					Name = "Test Classifier"
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success);
			Assert.AreNotEqual(Guid.Empty, result.Uid);
		}
	}
}

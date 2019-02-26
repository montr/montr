using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
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

			var handler = new InsertClassifierHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider);

			// act
			var command = new InsertClassifier
			{
				UserUid = Guid.NewGuid(),
				CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
				Item = new Classifier
				{
					TypeCode = "test",
					Code = "001",
					Name = "Test Classifier"
				}
			};

			var uid = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.AreNotEqual(Guid.Empty, uid);
		}
	}
}

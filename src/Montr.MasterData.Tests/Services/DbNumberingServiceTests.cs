using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class DbNumberingServiceTests
	{
		[TestMethod]
		public async Task GenerateNumber()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var service = new DbNumberingService(dbContextFactory, new NumeratorPatternParser());

			using (var _ = unitOfWorkFactory.Create())
			{
				var entityTypeCode = Classifier.EntityTypeCode;
				var enityUid = Guid.NewGuid();

				var numerator = await dbHelper.InsertNumerator(new Numerator
				{
					Pattern = "C-{Number}"
				}, cancellationToken);

				// ReSharper disable once PossibleInvalidOperationException
				var numeratorUid = numerator.Uid.Value;

				// act
				var number1 = await service.GenerateNumber(numeratorUid, entityTypeCode, enityUid, cancellationToken);
				var number2 = await service.GenerateNumber(numeratorUid, entityTypeCode, enityUid, cancellationToken);

				// assert
				Assert.AreEqual("C-00001", number1);
				Assert.AreEqual("C-00002", number2);
			}
		}
	}
}

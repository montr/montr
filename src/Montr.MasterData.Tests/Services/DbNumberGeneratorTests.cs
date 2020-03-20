using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class DbNumberGeneratorTests
	{
		[TestMethod]
		public async Task GenerateNumber_SimpleNumerator_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numberPatternParser = new NumberPatternParser();
			var numeratorTagProvider = new TestNumberTagProvider
			{
				EntityTypeCode = ClassifierType.EntityTypeCode,
				Values = new Dictionary<string, string> { { "{Company}", "MT" }, { "{Year}", "2020" } }
			};
			var service = new DbNumberGenerator(dbContextFactory, numberPatternParser, new INumberTagProvider[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var entityTypeCode = ClassifierType.EntityTypeCode;
				var enityUid = Guid.NewGuid();

				await dbHelper.InsertNumerator(new Numerator
				{
					Pattern = "{Company}-{Number}/{Year}"
				}, entityTypeCode, enityUid, cancellationToken);

				// act
				var number1 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);
				var number2 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);

				// assert
				Assert.AreEqual("MT-00001/2020", number1);
				Assert.AreEqual("MT-00002/2020", number2);
			}
		}

		[TestMethod]
		public async Task GenerateNumber_YearPeriodicity_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numberPatternParser = new NumberPatternParser();
			var numeratorTagProvider = new TestNumberTagProvider
			{
				EntityTypeCode = ClassifierType.EntityTypeCode,
				Values = new Dictionary<string, string>()
			};
			var service = new DbNumberGenerator(dbContextFactory, numberPatternParser, new INumberTagProvider[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var entityTypeCode = ClassifierType.EntityTypeCode;
				var enityUid = Guid.NewGuid();

				await dbHelper.InsertNumerator(new Numerator
				{
					Periodicity = NumeratorPeriodicity.Year,
					Pattern = "{Number}/{Year4}"
				}, entityTypeCode, enityUid, cancellationToken);

				// act - year 2020 - first time
				numeratorTagProvider.Date = new DateTime(2020, 05, 31);
				var number1Of2020 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);
				var number2Of2020 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);

				// assert - year 2020 - first time
				Assert.AreEqual("00001/2020", number1Of2020);
				Assert.AreEqual("00002/2020", number2Of2020);

				// act - year 2023
				numeratorTagProvider.Date = new DateTime(2023, 03, 13);
				var number1Of2023 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);
				var number2Of2023 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);

				// assert - year 2023
				Assert.AreEqual("00001/2023", number1Of2023);
				Assert.AreEqual("00002/2023", number2Of2023);

				// act - year 2020 - second time
				numeratorTagProvider.Date = new DateTime(2020, 10, 30);
				var number3Of2020 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);
				var number4Of2020 = await service.GenerateNumber(entityTypeCode, enityUid, cancellationToken);

				// assert - year 2020 - second time
				Assert.AreEqual("00003/2020", number3Of2020);
				Assert.AreEqual("00004/2020", number4Of2020);
			}
		}

		// todo: test uniquie tags in key
		// todo: test numbers digits parse
		// todo: test overflow of digits in number

		private class TestNumberTagProvider : INumberTagProvider
		{
			public string EntityTypeCode { get; set; }

			public IDictionary<string, string> Values { get; set; }

			public DateTime? Date { get; set; }

			public bool Supports(string entityTypeCode, out string[] supportedTags)
			{
				if (entityTypeCode == EntityTypeCode)
				{
					supportedTags = Values.Keys.ToArray();
					return true;
				}

				supportedTags = null;
				return false;
			}

			public Task Resolve(string entityTypeCode, Guid enityUid, out DateTime? date,
				IEnumerable<string> tags, IDictionary<string, string> values, CancellationToken cancellationToken)
			{
				date = Date;

				foreach (var tag in tags)
				{
					if (Values.TryGetValue(tag, out var value))
					{
						values[tag] = value;
					}
				}

				return Task.CompletedTask;
			}
		}
	}
}

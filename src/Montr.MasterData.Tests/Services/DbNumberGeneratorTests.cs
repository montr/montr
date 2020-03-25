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
		private static readonly string NumerableEntityTypeCode = "Numerable";

		[TestMethod]
		public async Task GenerateNumber_SimpleNumerator_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProvider = new TestNumberTagResolver { EntityTypeCode = NumerableEntityTypeCode };
			var service = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagResolver[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var request = new GenerateNumberRequest
				{
					EntityTypeCode = NumerableEntityTypeCode,
					EntityTypeUid = Guid.NewGuid()
				};

				await dbHelper.InsertNumerator(new Numerator
				{
					Pattern = "{Company}-{Number}/{Year}"
				}, request, cancellationToken);

				// act
				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "MT" }, { "Year", "2010" } };
				var number1 = await service.GenerateNumber(request, cancellationToken);

				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "GT" }, { "Year", "2020" } };
				var number2 = await service.GenerateNumber(request, cancellationToken);

				// assert
				Assert.AreEqual("MT-00001/2010", number1);
				Assert.AreEqual("GT-00002/2020", number2);
			}
		}

		[TestMethod]
		public async Task GenerateNumber_WithPeriodTags_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProvider = new TestNumberTagResolver { EntityTypeCode = NumerableEntityTypeCode };
			var service = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagResolver[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var request = new GenerateNumberRequest
				{
					EntityTypeCode = NumerableEntityTypeCode,
					EntityTypeUid = Guid.NewGuid()
				};

				await dbHelper.InsertNumerator(new Numerator
				{
					Periodicity = NumeratorPeriodicity.Year,
					Pattern = "{Company}-{Number}-{Day}-{Month}-{Quarter}-{Year2}-{Year4}"
				}, request, cancellationToken);

				// act
				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "MT" } };
				numeratorTagProvider.Date = new DateTime(2003, 2, 5);
				var number1 = await service.GenerateNumber(request, cancellationToken);
				numeratorTagProvider.Date = new DateTime(1999, 10, 30);
				var number2 = await service.GenerateNumber(request, cancellationToken);

				// assert
				Assert.AreEqual("MT-00001-05-02-1-03-2003", number1);
				Assert.AreEqual("MT-00001-30-10-4-99-1999", number2);
			}
		}

		[TestMethod]
		public async Task GenerateNumber_IndependentNumerator_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProvider = new TestNumberTagResolver { EntityTypeCode = NumerableEntityTypeCode };
			var service = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagResolver[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var request = new GenerateNumberRequest
				{
					EntityTypeCode = NumerableEntityTypeCode,
					EntityTypeUid = Guid.NewGuid()
				};

				await dbHelper.InsertNumerator(new Numerator
				{
					Pattern = "{Company}-{Number}",
					KeyTags = new[] { "Company" }
				}, request, cancellationToken);

				// act
				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "MT" } };
				var number1 = await service.GenerateNumber(request, cancellationToken);
				var number2 = await service.GenerateNumber(request, cancellationToken);

				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "GT" } };
				var number3 = await service.GenerateNumber(request, cancellationToken);
				var number4 = await service.GenerateNumber(request, cancellationToken);

				// assert
				Assert.AreEqual("MT-00001", number1);
				Assert.AreEqual("MT-00002", number2);
				Assert.AreEqual("GT-00001", number3);
				Assert.AreEqual("GT-00002", number4);
			}
		}

		[TestMethod]
		public async Task GenerateNumber_YearPeriodicity_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProvider = new TestNumberTagResolver
			{
				EntityTypeCode = NumerableEntityTypeCode,
				Values = new Dictionary<string, string>()
			};
			var service = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagResolver[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var request = new GenerateNumberRequest
				{
					EntityTypeCode = NumerableEntityTypeCode,
					EntityTypeUid = Guid.NewGuid()
				};

				await dbHelper.InsertNumerator(new Numerator
				{
					Periodicity = NumeratorPeriodicity.Year,
					Pattern = "{Company}-{Number}/{Year4}"
				}, request, cancellationToken);

				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "MT" } };

				// act - year 2020 - first time
				numeratorTagProvider.Date = new DateTime(2020, 05, 31);
				var number1Of2020 = await service.GenerateNumber(request, cancellationToken);
				var number2Of2020 = await service.GenerateNumber(request, cancellationToken);

				// assert - year 2020 - first time
				Assert.AreEqual("MT-00001/2020", number1Of2020);
				Assert.AreEqual("MT-00002/2020", number2Of2020);

				// act - year 2023
				numeratorTagProvider.Date = new DateTime(2023, 03, 13);
				var number1Of2023 = await service.GenerateNumber(request, cancellationToken);
				var number2Of2023 = await service.GenerateNumber(request, cancellationToken);

				// assert - year 2023
				Assert.AreEqual("MT-00001/2023", number1Of2023);
				Assert.AreEqual("MT-00002/2023", number2Of2023);

				// act - year 2020 - second time
				numeratorTagProvider.Date = new DateTime(2020, 10, 30);
				var number3Of2020 = await service.GenerateNumber(request, cancellationToken);
				var number4Of2020 = await service.GenerateNumber(request, cancellationToken);

				// assert - year 2020 - second time
				Assert.AreEqual("MT-00003/2020", number3Of2020);
				Assert.AreEqual("MT-00004/2020", number4Of2020);
			}
		}

		[TestMethod]
		public async Task GenerateNumber_QuarterPeriodicity_ShouldGenerate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProvider = new TestNumberTagResolver
			{
				EntityTypeCode = NumerableEntityTypeCode,
				Values = new Dictionary<string, string>()
			};
			var service = new DbNumberGenerator(dbContextFactory, dateTimeProvider, new INumberTagResolver[] { numeratorTagProvider });

			using (var _ = unitOfWorkFactory.Create())
			{
				var request = new GenerateNumberRequest
				{
					EntityTypeCode = NumerableEntityTypeCode,
					EntityTypeUid = Guid.NewGuid()
				};

				await dbHelper.InsertNumerator(new Numerator
				{
					Periodicity = NumeratorPeriodicity.Quarter,
					Pattern = "{Company}-{Number}/{Year4}"
				}, request, cancellationToken);

				numeratorTagProvider.Values = new Dictionary<string, string> { { "Company", "MT" } };

				// act - Q1.2020 - first time
				numeratorTagProvider.Date = new DateTime(2020, 01, 31);
				var number1OfQ1 = await service.GenerateNumber(request, cancellationToken);
				var number2OfQ1 = await service.GenerateNumber(request, cancellationToken);

				// assert - Q1.2020 - first time
				Assert.AreEqual("MT-00001/2020", number1OfQ1);
				Assert.AreEqual("MT-00002/2020", number2OfQ1);

				// act - Q3.2020
				numeratorTagProvider.Date = new DateTime(2020, 07, 30);
				var number1OfQ3 = await service.GenerateNumber(request, cancellationToken);
				var number2OfQ3 = await service.GenerateNumber(request, cancellationToken);

				// assert - Q3.2020
				Assert.AreEqual("MT-00001/2020", number1OfQ3);
				Assert.AreEqual("MT-00002/2020", number2OfQ3);

				// act - Q1.2020 - second time
				numeratorTagProvider.Date = new DateTime(2020, 02, 29);
				var number3OfQ1 = await service.GenerateNumber(request, cancellationToken);
				var number4OfQ1 = await service.GenerateNumber(request, cancellationToken);

				// assert - Q1.2020 - second time
				Assert.AreEqual("MT-00003/2020", number3OfQ1);
				Assert.AreEqual("MT-00004/2020", number4OfQ1);
			}
		}

		// todo: test case insensitive
		// todo: test numbers digits parse
		// todo: test overflow of digits in number

		private class TestNumberTagResolver : INumberTagResolver
		{
			public string EntityTypeCode { get; set; }

			public IDictionary<string, string> Values { get; set; }

			public DateTime? Date { get; set; }

			public bool Supports(GenerateNumberRequest request, out string[] supportedTags)
			{
				if (request.EntityTypeCode == EntityTypeCode)
				{
					supportedTags = Values.Keys.ToArray();
					return true;
				}

				supportedTags = null;
				return false;
			}

			public Task<NumberTagResolveResult> Resolve(GenerateNumberRequest request, IEnumerable<string> tags, CancellationToken cancellationToken)
			{
				var result = new NumberTagResolveResult
				{
					Date = Date,
					Values = Values.Where(x => tags.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value, Numerator.TagComparer)
				};

				return Task.FromResult(result);
			}
		}
	}
}

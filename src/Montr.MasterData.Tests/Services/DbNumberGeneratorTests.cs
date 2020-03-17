using System;
using System.Collections.Generic;
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
		public async Task GenerateNumber()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dbHelper = new DbHelper(unitOfWorkFactory, dbContextFactory);
			var numeratorTagProviders = new INumberTagProvider[ ] { new TestNumberTagProvider() };
			var service = new DbNumberGenerator(dbContextFactory, new NumberPatternParser(), numeratorTagProviders);

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

		private class TestNumberTagProvider : INumberTagProvider
		{
			public bool Supports(string entityTypeCode, out string[] supportedTags)
			{
				if (entityTypeCode == ClassifierType.EntityTypeCode)
				{
					supportedTags = new[] { "{Company}", "{Year}" };
					return true;
				}

				supportedTags = null;
				return false;
			}

			public Task Resolve(string entityTypeCode, Guid enityUid,
				IEnumerable<string> tags, IDictionary<string, string> values, CancellationToken cancellationToken)
			{
				foreach (var tag in tags)
				{
					switch (tag)
					{
						case "{Company}":
							values["{Company}"] = "MT";
							break;

						case "{Year}":
							values["{Year}"] = "2020";
							break;
					}
				}

				return Task.CompletedTask;
			}
		}
	}
}

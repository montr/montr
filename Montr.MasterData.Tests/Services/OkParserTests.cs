using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class OkParserTests
	{
		[TestMethod]
		public async Task Parser_Should_ParseOkeiFile()
		{
			// arrange
			var path = "../../../Content/nsiOkei_all_20190217_022439_001.xml";
			var parser = new OkeiParser();

			// act
			ICollection<Classifier> result;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				result = await parser.Parse(stream, CancellationToken.None);
			}

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(583, result.Count);
			Assert.AreEqual("728", result.Single(x => x.Name == "Пачка").Code);

			// await DumpToDb(result);
		}

		[TestMethod]
		public async Task Parser_Should_ParseOkved2File()
		{
			// arrange
			var path = "../../../Content/nsiOkved2_all_20190217_022436_001.xml";
			var parser = new Okved2Parser();

			// act
			ICollection<Classifier> result;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				result = await parser.Parse(stream, CancellationToken.None);
			}

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2794, result.Count);
			Assert.AreEqual("66.19.7", result.Single(x => x.Name == "Рейтинговая деятельность").Code);

			// await DumpToDb(result);
		}

		private static async Task DumpToDb(IEnumerable<Classifier> result)
		{
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();

			var handler = new InsertClassifierHandler(unitOfWorkFactory,
				dbContextFactory, dateTimeProvider);

			foreach (var classifier in result)
			{
				var command = new InsertClassifier
				{
					UserUid = Guid.NewGuid(),
					CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
					Item = classifier
				};

				await handler.Handle(command, CancellationToken.None);
			}
		}
	}
}
